using System;
using System.IO;
using System.Configuration;

namespace WordPressSecurityTrainingTool.Services
{
    public class SecurityLogger
    {
        private readonly string _logFilePath;
        private readonly bool _enableDetailedLogging;

        public SecurityLogger()
        {
            _logFilePath = ConfigurationManager.AppSettings["LogFilePath"] ?? "security_training.log";
            _enableDetailedLogging = bool.Parse(ConfigurationManager.AppSettings["EnableDetailedLogging"] ?? "true");
        }

        public void LogSecurityEvent(string message, string source)
        {
            try
            {
                var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{source}] {message}";
                
                if (_enableDetailedLogging)
                {
                    Console.WriteLine(logEntry);
                }
                
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка записи в лог: {ex.Message}");
            }
        }
    }
}
