using System;
using System.Threading.Tasks;
using WordPressSecurityTrainingTool.Core;
using WordPressSecurityTrainingTool.Models;
using WordPressSecurityTrainingTool.Services;
using WordPressSecurityTrainingTool.Utilities;

namespace WordPressSecurityTrainingTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== УЧЕБНЫЙ ИНСТРУМЕНТ БЕЗОПАСНОСТИ WORDPRESS ===");
                Console.WriteLine("Только для тестовых сред с явного разрешения\n");
                
                // Проверка среды выполнения
                if (!EnvironmentChecker.IsTestEnvironment())
                {
                    Console.WriteLine("ОШИБКА: Инструмент может использоваться только в тестовых средах");
                    Environment.Exit(1);
                    return;
                }

                // Загрузка конфигурации
                var config = TrainingSessionManager.LoadConfiguration();
                var logger = new SecurityLogger();
                
                logger.LogSecurityEvent("Сессия обучения начата", "Instructor");
                
                // Демонстрация уязвимостей
                await TrainingSessionManager.RunSecurityDemonstration(config);
                
                logger.LogSecurityEvent("Сессия обучения завершена", "Instructor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}
