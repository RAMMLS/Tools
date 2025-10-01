using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IWshRuntimeLibrary;
using RandomShortcutCreator.Models;
using RandomShortcutCreator.Utilities;

namespace RandomShortcutCreator.Services
{
    public class ShortcutService
    {
        private readonly RandomGenerator _randomGenerator;
        private readonly IconService _iconService;
        private readonly List<string> _createdShortcuts;

        public ShortcutService()
        {
            _randomGenerator = new RandomGenerator();
            _iconService = new IconService();
            _createdShortcuts = new List<string>();
        }

        public List<ShortcutResult> CreateRandomShortcuts(ShortcutConfig config)
        {
            var results = new List<ShortcutResult>();

            for (int i = 0; i < config.Count; i++)
            {
                var result = CreateSingleShortcut(config, i + 1);
                results.Add(result);

                if (result.Success)
                {
                    _createdShortcuts.Add(result.FilePath);
                }
            }

            return results;
        }

        private ShortcutResult CreateSingleShortcut(ShortcutConfig config, int index)
        {
            try
            {
                // Определение целевого пути
                string targetPath = GetRandomTargetPath(config);
                string shortcutName = GetShortcutName(config, index);
                string shortcutPath = GetShortcutFilePath(shortcutName, config.DesktopOnly);

                // Создание ярлыка
                CreateShortcutFile(shortcutPath, targetPath, config.UseRandomIcons);

                return new ShortcutResult
                {
                    Success = true,
                    FilePath = shortcutPath,
                    TargetPath = targetPath
                };
            }
            catch (Exception ex)
            {
                return new ShortcutResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private string GetRandomTargetPath(ShortcutConfig config)
        {
            return config.TargetType switch
            {
                "Application" => _randomGenerator.GetRandomApplication(),
                "Website" => _randomGenerator.GetRandomWebsite(),
                "Document" => _randomGenerator.GetRandomDocument(),
                "Folder" => _randomGenerator.GetRandomFolder(),
                _ => "notepad.exe"
            };
        }

        private string GetShortcutName(ShortcutConfig config, int index)
        {
            if (config.UseRandomNames)
            {
                return _randomGenerator.GenerateRandomName();
            }
            else
            {
                return config.CustomNamePattern.Replace("{number}", index.ToString());
            }
        }

        private string GetShortcutFilePath(string name, bool desktopOnly)
        {
            string directory = desktopOnly ? 
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop) :
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string safeName = FileHelper.MakeValidFileName(name);
            return Path.Combine(directory, $"{safeName}.lnk");
        }

        private void CreateShortcutFile(string shortcutPath, string targetPath, bool useRandomIcon)
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath) ?? string.Empty;
            shortcut.Description = $"Shortcut to {Path.GetFileName(targetPath)}";
            
            if (useRandomIcon)
            {
                string iconPath = _iconService.GetRandomIcon();
                if (!string.IsNullOrEmpty(iconPath))
                {
                    shortcut.IconLocation = iconPath;
                }
            }

            shortcut.Save();
        }

        public int ClearCreatedShortcuts()
        {
            int deletedCount = 0;

            foreach (var shortcutPath in _createdShortcuts.ToList())
            {
                try
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                        deletedCount++;
                    }
                    _createdShortcuts.Remove(shortcutPath);
                }
                catch
                {
                    // Игнорируем ошибки удаления
                }
            }

            return deletedCount;
        }
    }
}
