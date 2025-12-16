using System;
using System.IO;
using PowerShellPayloadGenerator.Models;
using PowerShellPayloadGenerator.Services;

namespace PowerShellPayloadGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Генератор PowerShell Полезных Нагрузок ===");
            Console.WriteLine("ТОЛЬКО ДЛЯ ОБРАЗОВАТЕЛЬНЫХ И ЗАКОННЫХ ЦЕЛЕЙ!\n");
            
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            try
            {
                var config = ParseArguments(args);
                Console.WriteLine($"Конфигурация: {config}");
                
                var service = new PayloadService();
                var result = service.GeneratePayload(config);
                
                if (result.Success)
                {
                    DisplayResult(result);
                    
                    // Сохранение в файл
                    if (ShouldSaveToFile())
                    {
                        SavePayloadToFile(result);
                    }
                }
                else
                {
                    DisplayErrors(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ОШИБКА] {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static PayloadConfig ParseArguments(string[] args)
        {
            var config = new PayloadConfig();
            
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--type":
                        if (i + 1 < args.Length) config.Type = args[++i];
                        break;
                    case "--ip":
                        if (i + 1 < args.Length) config.IpAddress = args[++i];
                        break;
                    case "--port":
                        if (i + 1 < args.Length) config.Port = int.Parse(args[++i]);
                        break;
                    case "--command":
                        if (i + 1 < args.Length) config.Command = args[++i];
                        break;
                    case "--encode":
                        config.Encode = true;
                        break;
                    case "--obfuscate":
                        config.Obfuscate = true;
                        break;
                    case "--validate":
                        config.Validate = true;
                        break;
                    case "--timeout":
                        if (i + 1 < args.Length) config.Timeout = int.Parse(args[++i]);
                        break;
                    case "--https":
                        config.UseHttps = true;
                        break;
                }
            }
            
            return config;
        }

        private static void DisplayResult(GenerationResult result)
        {
            Console.WriteLine($"\n=== РЕЗУЛЬТАТ ГЕНЕРАЦИИ ===");
            Console.WriteLine($"Статус: УСПЕШНО");
            Console.WriteLine($"Уровень риска: {result.RiskLevel}");
            Console.WriteLine($"Размер: {result.Size} байт");
            Console.WriteLine($"Время генерации: {result.GenerationTimeMs} мс");
            Console.WriteLine($"Хэш SHA256: {result.Hash}");
            
            if (result.Warnings.Count > 0)
            {
                Console.WriteLine($"\nПредупреждения:");
                foreach (var warning in result.Warnings)
                {
                    Console.WriteLine($"  ⚠ {warning}");
                }
            }
            
            Console.WriteLine($"\n=== СГЕНЕРИРОВАННАЯ НАГРУЗКА ===");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine(result.Payload);
            Console.WriteLine(new string('=', 80));
        }

        private static void DisplayErrors(GenerationResult result)
        {
            Console.WriteLine($"\n=== ОШИБКА ГЕНЕРАЦИИ ===");
            Console.WriteLine($"Уровень риска: {result.RiskLevel}");
            
            if (result.Errors.Count > 0)
            {
                Console.WriteLine($"Ошибки:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  ❌ {error}");
                }
            }
            
            Environment.Exit(1);
        }

        private static bool ShouldSaveToFile()
        {
            Console.Write("\nСохранить нагрузку в файл? (y/n): ");
            var response = Console.ReadLine()?.Trim().ToLower();
            return response == "y" || response == "yes";
        }

        private static void SavePayloadToFile(GenerationResult result)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"payload_{timestamp}.ps1";
            
            File.WriteAllText(filename, result.Payload);
            Console.WriteLine($"Нагрузка сохранена в файл: {filename}");
            
            // Также сохраняем метаданные
            var metadataFile = $"payload_{timestamp}_metadata.txt";
            using (var writer = new StreamWriter(metadataFile))
            {
                writer.WriteLine("=== МЕТАДАННЫЕ ПОЛЕЗНОЙ НАГРУЗКИ ===");
                writer.WriteLine($"Дата создания: {result.Timestamp:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Уровень риска: {result.RiskLevel}");
                writer.WriteLine($"Размер: {result.Size} байт");
                writer.WriteLine($"Хэш SHA256: {result.Hash}");
                writer.WriteLine($"Время генерации: {result.GenerationTimeMs} мс");
                
                if (result.Metadata.Count > 0)
                {
                    writer.WriteLine("\nДополнительные метаданные:");
                    foreach (var kvp in result.Metadata)
                    {
                        writer.WriteLine($"  {kvp.Key}: {kvp.Value}");
                    }
                }
            }
            Console.WriteLine($"Метаданные сохранены в файл: {metadataFile}");
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Использование:");
            Console.WriteLine("  --type <type>      Тип нагрузки [Command, Discovery, SecurityAudit, ...]");
            Console.WriteLine("  --ip <address>     IP-адрес цели");
            Console.WriteLine("  --port <port>      Порт цели (1-65535)");
            Console.WriteLine("  --command <cmd>    Команда для выполнения");
            Console.WriteLine("  --encode           Кодировать в Base64");
            Console.WriteLine("  --obfuscate        Обфусцировать код");
            Console.WriteLine("  --validate         Валидировать синтаксис");
            Console.WriteLine("  --timeout <sec>    Таймаут в секундах");
            Console.WriteLine("  --https            Использовать HTTPS");
            Console.WriteLine("\nПримеры:");
            Console.WriteLine("  dotnet run -- --type Discovery");
            Console.WriteLine("  dotnet run -- --type SecurityAudit");
            Console.WriteLine("  dotnet run -- --type Command --command \"Get-Service\"");
            Console.WriteLine("\nДоступные типы нагрузок:");
            Console.WriteLine("  Command              - Выполнение команды");
            Console.WriteLine("  Discovery           - Обнаружение информации");
            Console.WriteLine("  SecurityAudit       - Проверка безопасности");
            Console.WriteLine("  PrivilegeEnumeration- Перечисление привилегий");
            Console.WriteLine("  VulnerabilityCheck  - Проверка уязвимостей");
            Console.WriteLine("  PortScan            - Сканирование портов");
            Console.WriteLine("  LogCollection       - Сбор логов");
            Console.WriteLine("\nПРЕДУПРЕЖДЕНИЕ: Используйте только в законных целях с явного разрешения!");
        }
    }
}
