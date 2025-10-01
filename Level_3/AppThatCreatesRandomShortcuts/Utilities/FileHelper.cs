using System;
using System.IO;
using System.Linq;

namespace RandomShortcutCreator.Utilities
{
    public static class FileHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        public static string MakeValidFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Shortcut";

            // Удаляем недопустимые символы
            var validName = new string(name
                .Where(ch => !InvalidChars.Contains(ch))
                .ToArray());

            // Убеждаемся, что имя не пустое после очистки
            if (string.IsNullOrWhiteSpace(validName))
                validName = "Shortcut";

            // Обрезаем до разумной длины
            return validName.Length > 50 ? validName.Substring(0, 50) : validName;
        }

        public static bool IsValidPath(string path)
        {
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
