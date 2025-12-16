using System.Collections.Generic;

namespace PowerShellPayloadGenerator.Models
{
    /// <summary>
    /// Контекст безопасности для контроля доступа
    /// </summary>
    public class SecurityContext
    {
        /// <summary>
        /// Разрешенные типы нагрузок
        /// </summary>
        public List<string> AllowedPayloadTypes { get; set; } = new List<string>
        {
            "Command",
            "Discovery",
            "SecurityAudit",
            "LogCollection"
        };

        /// <summary>
        /// Запрещенные типы нагрузок
        /// </summary>
        public List<string> RestrictedPayloadTypes { get; set; } = new List<string>
        {
            "ReverseShell",
            "Download"
        };

        /// <summary>
        /// Разрешенные IP диапазоны
        /// </summary>
        public List<string> AllowedIpRanges { get; set; } = new List<string>
        {
            "192.168.0.0/16",
            "10.0.0.0/8",
            "127.0.0.1"
        };

        /// <summary>
        /// Запрещенные команды
        /// </summary>
        public List<string> ForbiddenCommands { get; set; } = new List<string>
        {
            "Format-Volume",
            "Remove-Item -Recurse -Force",
            "Stop-Service -Name WinRM -Force",
            "Set-ExecutionPolicy Unrestricted"
        };

        /// <summary>
        /// Максимальный размер полезной нагрузки
        /// </summary>
        public int MaxPayloadSize { get; set; } = 1048576; // 1MB

        /// <summary>
        /// Требовать авторизацию
        /// </summary>
        public bool RequireAuthorization { get; set; } = true;

        /// <summary>
        /// Включить аудит
        /// </summary>
        public bool EnableAudit { get; set; } = true;

        /// <summary>
        /// Проверить разрешение типа
        /// </summary>
        public bool IsPayloadTypeAllowed(string payloadType)
        {
            return AllowedPayloadTypes.Contains(payloadType);
        }

        /// <summary>
        /// Проверить разрешение IP адреса
        /// </summary>
        public bool IsIpAllowed(string ipAddress)
        {
            // Базовая проверка - всегда разрешаем localhost для демо
            if (ipAddress == "127.0.0.1" || ipAddress == "localhost")
                return true;

            // Здесь должна быть логика проверки CIDR диапазонов
            return true;
        }

        /// <summary>
        /// Проверить команду на наличие запрещенных паттернов
        /// </summary>
        public bool IsCommandAllowed(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return true;

            foreach (var forbidden in ForbiddenCommands)
            {
                if (command.Contains(forbidden))
                    return false;
            }

            return true;
        }
    }
}
