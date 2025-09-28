using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ARPPoisoningTool.Utilities
{
    public class Logger : IDisposable
    {
        private readonly string _logFilePath;
        private readonly StreamWriter _writer;
        private readonly List<string> _logBuffer;

        public Logger(string filePath)
        {
            _logFilePath = filePath;
            _logBuffer = new List<string>();
            
            try
            {
                _writer = new StreamWriter(filePath, append: true);
                LogInfo("Logger initialized");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize logger: {ex.Message}");
            }
        }

        public void LogInfo(string message)
        {
            var logEntry = $"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            WriteLog(logEntry);
        }

        public void LogError(string message)
        {
            var logEntry = $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            WriteLog(logEntry);
        }

        public void LogWarning(string message)
        {
            var logEntry = $"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            WriteLog(logEntry);
        }

        private void WriteLog(string logEntry)
        {
            Console.WriteLine(logEntry);
            _logBuffer.Add(logEntry);
            
            if (_logBuffer.Count > 1000)
            {
                _logBuffer.RemoveAt(0);
            }

            try
            {
                _writer?.WriteLine(logEntry);
                _writer?.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }

        public List<string> GetRecentLogs(int count)
        {
            return _logBuffer.TakeLast(count).ToList();
        }

        public void ClearLogs()
        {
            _logBuffer.Clear();
        }

        public void Dispose()
        {
            _writer?.Close();
            _writer?.Dispose();
        }
    }
}
