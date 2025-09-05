using System;
using System.IO;

namespace MysqlUserFootprint
{
    public static class Logger
    {
        private static string logFilePath;
        private static StreamWriter logFile;

        public static void Initialize(string filePath)
        {
            logFilePath = filePath;
            try
            {
                logFile = new StreamWriter(logFilePath, true); // true - append
                logFile.AutoFlush = true; //  Автоматически сбрасывать буфер
                LogInfo("Логгер инициализирован.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка инициализации логгера: " + ex.Message);
            }
        }

        public static void LogInfo(string message)
        {
            Log($"[INFO] {DateTime.Now}: {message}");
        }

        public static void LogError(string message)
        {
            Log($"[ERROR] {DateTime.Now}: {message}");
        }

        private static void Log(string message)
        {
            if (logFile != null)
            {
                try
                {
                    logFile.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка записи в лог: " + ex.Message);
                }
            }
        }

        public static void Close()
        {
            if (logFile != null)
            {
                try
                {
                    LogInfo("Логгер закрыт.");
                    logFile.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка закрытия логгера: " + ex.Message);
                }
            }
        }
    }
}

