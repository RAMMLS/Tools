using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using WordPressSecurityTrainingTool.Models;
using WordPressSecurityTrainingTool.Services;
using WordPressSecurityTrainingTool.Utilities;

namespace WordPressSecurityTrainingTool.Core
{
    public static class TrainingSessionManager
    {
        public static TrainingSessionConfig LoadConfiguration()
        {
            return new TrainingSessionConfig
            {
                AllowedTestDomains = ConfigurationManager.AppSettings["AllowedTestDomains"]?.Split(','),
                RequestDelayMs = int.Parse(ConfigurationManager.AppSettings["RequestDelayMs"] ?? "2000"),
                MaxDemoAttempts = int.Parse(ConfigurationManager.AppSettings["MaxDemoAttempts"] ?? "5"),
                LogFilePath = ConfigurationManager.AppSettings["LogFilePath"] ?? "security_training.log",
                EnableDetailedLogging = bool.Parse(ConfigurationManager.AppSettings["EnableDetailedLogging"] ?? "true")
            };
        }

        public static async Task RunSecurityDemonstration(TrainingSessionConfig config)
        {
            var client = new TestWordPressClient();
            var analyzer = new PasswordStrengthAnalyzer();
            var logger = new SecurityLogger();
            
            Console.WriteLine("Демонстрация уязвимостей безопасности WordPress:");
            Console.WriteLine("================================================");
            
            // Демонстрация слабых паролей
            Console.WriteLine("\n1. Тестирование слабых паролей:");
            var weakPasswords = new[] { "password", "123456", "admin", "qwerty", "letmein" };
            
            foreach (var password in weakPasswords)
            {
                var strength = analyzer.TestPasswordStrength(password);
                Console.WriteLine($"Пароль: {password} - Сложность: {strength.Score}/5 - Рекомендация: {strength.Recommendation}");
                
                // Имитация попытки входа (без реального запроса)
                var result = await client.TestLoginAsync("http://test.example/wp-login.php", "admin", password);
                Console.WriteLine($"Результат: {result.Lesson}\n");
            }
            
            // Демонстрация защиты от brute-force
            Console.WriteLine("\n2. Демонстрация защиты от brute-force атак:");
            Console.WriteLine("Имитация ограничения попыток входа...");
            
            for (int i = 0; i < config.MaxDemoAttempts; i++)
            {
                var result = await client.TestLoginAsync("http://test.example/wp-login.php", "admin", "wrongpassword");
                Console.WriteLine($"Попытка {i + 1}: {result.Lesson}");
                
                if (result.IsLockoutSimulated)
                {
                    Console.WriteLine("СИМУЛЯЦИЯ: Обнаружена защита от brute-force - аккаунт временно заблокирован");
                    break;
                }
            }
            
            Console.WriteLine("\n3. Рекомендации по безопасности:");
            Console.WriteLine("- Используйте сложные пароли (12+ символов, разные регистры, цифры, спецсимволы)");
            Console.WriteLine("- Ограничьте попытки входа с одного IP-адреса");
            Console.WriteLine("- Внедрите двухфакторную аутентификацию");
            Console.WriteLine("- Регулярно обновляйте WordPress и плагины");
            Console.WriteLine("- Используйте плагины безопасности (Wordfence, Sucuri, iThemes Security)");
            
            logger.LogSecurityEvent("Демонстрация завершена", "System");
        }
    }
}
