using System;
using System.Collections.Generic;
using System.Text;

namespace PowerShellPayloadGenerator.Models
{
    /// <summary>
    /// Конфигурация для генерации полезной нагрузки
    /// </summary>
    public class PayloadConfig
    {
        /// <summary>
        /// Тип полезной нагрузки
        /// </summary>
        public string Type { get; set; } = "Command";

        /// <summary>
        /// IP адрес цели
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Порт цели
        /// </summary>
        public int Port { get; set; } = 443;

        /// <summary>
        /// Команда для выполнения
        /// </summary>
        public string Command { get; set; } = "Get-Process";

        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Кодировать в Base64
        /// </summary>
        public bool Encode { get; set; } = false;

        /// <summary>
        /// Обфусцировать код
        /// </summary>
        public bool Obfuscate { get; set; } = false;

        /// <summary>
        /// Валидировать синтаксис
        /// </summary>
        public bool Validate { get; set; } = true;

        /// <summary>
        /// Таймаут в секундах
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Использовать HTTPS
        /// </summary>
        public bool UseHttps { get; set; } = false;

        /// <summary>
        /// Дополнительные параметры
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Проверка конфигурации
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Type))
                return false;

            if (Type.Equals("ReverseShell", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(IpAddress))
                    return false;
                if (Port < 1 || Port > 65535)
                    return false;
            }

            if (Type.Equals("Download", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(IpAddress) && string.IsNullOrWhiteSpace(FilePath))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Получить строковое представление
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Type: {Type}");
            sb.AppendLine($"IP Address: {IpAddress ?? "Not set"}");
            sb.AppendLine($"Port: {Port}");
            sb.AppendLine($"Command: {Command}");
            sb.AppendLine($"Encode: {Encode}");
            sb.AppendLine($"Obfuscate: {Obfuscate}");
            sb.AppendLine($"Validate: {Validate}");
            sb.AppendLine($"Timeout: {Timeout}s");
            sb.AppendLine($"Use HTTPS: {UseHttps}");
            
            if (Parameters.Count > 0)
            {
                sb.AppendLine("Parameters:");
                foreach (var param in Parameters)
                {
                    sb.AppendLine($"  {param.Key}: {param.Value}");
                }
            }

            return sb.ToString();
        }
    }
}
