using System;
using System.Linq;
using System.Text;

namespace AsciiNameGenerator.Utilities
{
    public static class TextHelper
    {
        public static string CenterText(string text, int width)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var lines = text.Split('\n');
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                var padding = Math.Max(0, (width - line.Length) / 2);
                result.AppendLine(new string(' ', padding) + line);
            }

            return result.ToString();
        }

        public static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        public static bool IsValidAsciiText(string text)
        {
            // Проверяем, содержит ли текст только ASCII символы
            return text.All(c => c <= 127);
        }

        public static string AddTextEffects(string text, string effect)
        {
            return effect.ToLower() switch
            {
                "shadow" => AddShadowEffect(text),
                "outline" => AddOutlineEffect(text),
                _ => text
            };
        }

        private static string AddShadowEffect(string text)
        {
            // Упрощенный эффект тени
            var lines = text.Split('\n');
            var result = new StringBuilder();

            foreach (var line in lines)
            {
                result.AppendLine(line + " ░");
            }

            return result.ToString();
        }

        private static string AddOutlineEffect(string text)
        {
            // Упрощенный эффект контура
            return text;
        }
    }
}
