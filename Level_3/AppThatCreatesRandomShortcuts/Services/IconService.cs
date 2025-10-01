using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RandomShortcutCreator.Services
{
    public class IconService
    {
        private readonly List<string> _systemIcons;
        private readonly Random _random;

        public IconService()
        {
            _random = new Random();
            _systemIcons = FindSystemIcons();
        }

        public string GetRandomIcon()
        {
            if (_systemIcons.Count == 0) return string.Empty;
            
            return _systemIcons[_random.Next(_systemIcons.Count)];
        }

        private List<string> FindSystemIcons()
        {
            var icons = new List<string>();
            string systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.System);

            try
            {
                // Стандартные системные исполняемые файлы с иконками
                var executableFiles = new[]
                {
                    Path.Combine(systemRoot, "shell32.dll"),
                    Path.Combine(systemRoot, "imageres.dll"),
                    Path.Combine(systemRoot, "notepad.exe"),
                    Path.Combine(systemRoot, "calc.exe"),
                    Path.Combine(systemRoot, "mspaint.exe"),
                    Path.Combine(systemRoot, "write.exe"),
                    Path.Combine(systemRoot, "explorer.exe")
                };

                foreach (var file in executableFiles)
                {
                    if (File.Exists(file))
                    {
                        icons.Add(file);
                    }
                }

                // Добавляем дополнительные пути
                var additionalPaths = new[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                };

                foreach (var path in additionalPaths)
                {
                    if (Directory.Exists(path))
                    {
                        // Ищем EXE файлы в общих местах
                        var commonFolders = new[] { "Internet Explorer", "Windows Media Player", "Windows NT" };
                        foreach (var folder in commonFolders)
                        {
                            var fullPath = Path.Combine(path, folder);
                            if (Directory.Exists(fullPath))
                            {
                                var exeFiles = Directory.GetFiles(fullPath, "*.exe", SearchOption.TopDirectoryOnly);
                                icons.AddRange(exeFiles.Take(5)); // Берем только несколько файлов
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // В случае ошибки возвращаем базовый набор
                icons.Add(Path.Combine(systemRoot, "notepad.exe"));
            }

            return icons.Distinct().ToList();
        }
    }
}
