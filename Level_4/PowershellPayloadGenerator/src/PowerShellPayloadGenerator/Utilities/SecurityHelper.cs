using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PowerShellPayloadGenerator.Utilities
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Кодирует строку в Base64
        /// </summary>
        public static string EncodeBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Декодирует строку из Base64
        /// </summary>
        public static string DecodeBase64(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            try
            {
                var bytes = Convert.FromBase64String(input);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Вычисляет SHA256 хэш строки
        /// </summary>
        public static string CalculateSha256Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Обфусцирует PowerShell скрипт
        /// </summary>
        public static string ObfuscatePowerShell(string script)
        {
            if (string.IsNullOrEmpty(script))
                return script;

            // Простая обфускация - разбиваем строки и меняем имена переменных
            var obfuscated = script;
            
            // Заменяем имена переменных
            obfuscated = Regex.Replace(obfuscated, @"\bclient\b", "c1");
            obfuscated = Regex.Replace(obfuscated, @"\bstream\b", "s1");
            obfuscated = Regex.Replace(obfuscated, @"\bbytes\b", "b1");
            obfuscated = Regex.Replace(obfuscated, @"\bdata\b", "d1");
            
            // Удаляем лишние пробелы и переносы строк
            obfuscated = Regex.Replace(obfuscated, @"\s+", " ");
            obfuscated = Regex.Replace(obfuscated, @"\r\n", "");
            
            // Разбиваем длинные строки
            obfuscated = Regex.Replace(obfuscated, @"(?<=;) ", "`n");
            
            return obfuscated;
        }

        /// <summary>
        /// Проверяет валидность PowerShell синтаксиса
        /// </summary>
        public static bool ValidatePowerShellSyntax(string script)
        {
            if (string.IsNullOrEmpty(script))
                return false;

            try
            {
                // Проверяем наличие опасных команд
                var dangerousPatterns = new[]
                {
                    @"Remove-Item.*-Recurse.*-Force",
                    @"Format-Volume",
                    @"Stop-Service.*-Force",
                    @"Set-ExecutionPolicy.*Unrestricted",
                    @"Invoke-Expression.*http",
                    @"IEX.*http"
                };

                foreach (var pattern in dangerousPatterns)
                {
                    if (Regex.IsMatch(script, pattern, RegexOptions.IgnoreCase))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Генерирует случайную строку
        /// </summary>
        public static string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Проверяет валидность IP адреса
        /// </summary>
        public static bool IsValidIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            return Regex.IsMatch(ipAddress, 
                @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");
        }

        /// <summary>
        /// Проверяет валидность порта
        /// </summary>
        public static bool IsValidPort(int port)
        {
            return port > 0 && port <= 65535;
        }

        /// <summary>
        /// Маскирует чувствительные данные в логах
        /// </summary>
        public static string MaskSensitiveData(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Маскируем IP адреса
            var masked = Regex.Replace(input, 
                @"\b(?:\d{1,3}\.){3}\d{1,3}\b", 
                "***.***.***.***");
            
            // Маскируем пароли
            masked = Regex.Replace(masked,
                @"(password|pwd|pass)[\s]*[=:][\s]*([^\s]+)",
                "$1=***MASKED***",
                RegexOptions.IgnoreCase);
            
            return masked;
        }
    }
}
