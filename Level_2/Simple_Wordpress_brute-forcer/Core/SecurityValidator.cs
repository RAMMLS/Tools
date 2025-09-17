using System;
using System.Text.RegularExpressions;

namespace WordPressSecurityTrainingTool.Core
{
    public static class SecurityValidator
    {
        public static void ValidateTarget(string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("URL не может быть пустым");
            
            if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out Uri uri))
                throw new ArgumentException("Некорректный URL");
            
            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new ArgumentException("Поддерживаются только HTTP и HTTPS протоколы");
        }

        public static void ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Имя пользователя не может быть пустым");
            
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым");
            
            // Проверка на потенциально опасные payloads
            if (ContainsSqlInjectionPattern(username) || ContainsSqlInjectionPattern(password))
                throw new ArgumentException("Обнаружены потенциально опасные символы");
        }

        private static bool ContainsSqlInjectionPattern(string input)
        {
            var patterns = new[]
            {
                "--", ";", "\"", "'", "/*", "*/", "@@", "@", 
                "char(", "nchar(", "exec", "execute", "select", 
                "insert", "update", "delete", "drop", "create", "alter"
            };
            
            foreach (var pattern in patterns)
            {
                if (input.ToLower().Contains(pattern))
                    return true;
            }
            
            return false;
        }
    }
}
