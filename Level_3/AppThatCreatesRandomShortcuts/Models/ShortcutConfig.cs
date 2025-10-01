using System.Collections.Generic;

namespace RandomShortcutCreator.Models
{
    public class ShortcutConfig
    {
        public int Count { get; set; } = 5;
        public string TargetType { get; set; } = "Application";
        public bool UseRandomNames { get; set; } = true;
        public bool UseRandomIcons { get; set; } = true;
        public bool DesktopOnly { get; set; } = true;
        public string CustomNamePattern { get; set; } = "Shortcut_{number}";
        public List<string> TargetApplications { get; set; } = new List<string>
        {
            "notepad.exe",
            "calc.exe",
            "mspaint.exe",
            "cmd.exe",
            "explorer.exe"
        };
        public List<string> TargetWebsites { get; set; } = new List<string>
        {
            "https://www.google.com",
            "https://www.github.com",
            "https://stackoverflow.com",
            "https://www.microsoft.com"
        };
    }

    public class ShortcutResult
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public string TargetPath { get; set; }
        public string ErrorMessage { get; set; }
    }
}
