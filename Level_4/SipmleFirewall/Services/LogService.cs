using System.Collections.Concurrent;
using SimpleFirewall.Models;

namespace SimpleFirewall.Services
{
    public class LogService
    {
        private readonly string _logDirectory;
        private readonly ConcurrentQueue<string> _logQueue;
        private readonly Timer _flushTimer;
        private readonly object _fileLock = new();

        public LogService()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);

            _logQueue = new ConcurrentQueue<string>();
            _flushTimer = new Timer(FlushLogs, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public void LogInfo(string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [INFO] {message}";
            _logQueue.Enqueue(logEntry);
            Console.WriteLine(logEntry);
        }

        public void LogWarning(string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [WARN] {message}";
            _logQueue.Enqueue(logEntry);
            Console.WriteLine(logEntry);
        }

        public void LogError(string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] {message}";
            _logQueue.Enqueue(logEntry);
            Console.WriteLine(logEntry);
        }

        public void LogPacket(NetworkPacket packet, RuleAction action)
        {
            var actionStr = action == RuleAction.Allow ? "ALLOW" : "BLOCK";
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{actionStr}] {packet.SourceAddress}:{packet.SourcePort} -> {packet.DestinationAddress}:{packet.DestinationPort} ({packet.Protocol})";
            _logQueue.Enqueue(logEntry);
        }

        private void FlushLogs(object? state)
        {
            var logFile = Path.Combine(_logDirectory, $"firewall_{DateTime.Now:yyyyMMdd}.log");
            var entries = new List<string>();

            while (_logQueue.TryDequeue(out var entry))
            {
                entries.Add(entry);
            }

            if (entries.Count > 0)
            {
                lock (_fileLock)
                {
                    File.AppendAllLines(logFile, entries);
                }
            }
        }

        public List<string> GetRecentLogs(int count = 100)
        {
            var logFile = Path.Combine(_logDirectory, $"firewall_{DateTime.Now:yyyyMMdd}.log");
            if (File.Exists(logFile))
            {
                lock (_fileLock)
                {
                    var lines = File.ReadAllLines(logFile);
                    return lines.TakeLast(count).ToList();
                }
            }
            return new List<string>();
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();
            FlushLogs(null); // Финальный flush
        }
    }
}
