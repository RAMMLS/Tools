using System;
using System.IO;
using System.Text.Json;

namespace RandomShortcutCreator.Models
{
    public class ApplicationSettings
    {
        public string DefaultShortcutLocation { get; set; } = "Desktop";
        public bool CreateBackup { get; set; } = true;
        public bool EnableLogging { get; set; } = true;
        public string LogFilePath { get; set; } = "shortcuts.log";
        public int MaxShortcutsPerSession { get; set; } = 20;
        public string[] AllowedFileTypes { get; set; } = { ".exe", ".url", ".lnk", ".txt", ".doc" };
        
        private static readonly string SettingsPath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                        "RandomShortcutCreator", "settings.json");

        public static ApplicationSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings();
                }
            }
            catch (Exception)
            {
                // Если ошибка загрузки, возвращаем настройки по умолчанию
            }
            
            return new ApplicationSettings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save settings: {ex.Message}");
            }
        }
    }
}
