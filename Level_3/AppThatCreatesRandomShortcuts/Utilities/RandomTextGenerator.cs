using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomShortcutCreator.Utilities
{
    public class RandomGenerator
    {
        private readonly Random _random;
        private readonly List<string> _applications;
        private readonly List<string> _websites;
        private readonly List<string> _documents;
        private readonly List<string> _folders;
        private readonly List<string> _adjectives;
        private readonly List<string> _nouns;

        public RandomGenerator()
        {
            _random = new Random();
            
            _applications = new List<string>
            {
                "notepad.exe", "calc.exe", "mspaint.exe", "cmd.exe", "explorer.exe",
                "write.exe", "snippingtool.exe", "taskmgr.exe", "control.exe"
            };

            _websites = new List<string>
            {
                "https://www.google.com", "https://www.github.com", "https://stackoverflow.com",
                "https://www.microsoft.com", "https://www.youtube.com", "https://www.reddit.com"
            };

            _documents = new List<string>
            {
                @"C:\Windows\System32\drivers\etc\hosts",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            _folders = new List<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            _adjectives = new List<string>
            {
                "Quick", "Smart", "Fast", "Powerful", "Amazing", "Incredible", "Fantastic",
                "Super", "Mega", "Ultra", "Digital", "Virtual", "Cloud", "AI", "Smart"
            };

            _nouns = new List<string>
            {
                "Tool", "App", "Utility", "Manager", "Assistant", "Helper", "Generator",
                "Creator", "Master", "Expert", "System", "Platform", "Solution", "Device"
            };
        }

        public string GetRandomApplication()
        {
            return _applications[_random.Next(_applications.Count)];
        }

        public string GetRandomWebsite()
        {
            return _websites[_random.Next(_websites.Count)];
        }

        public string GetRandomDocument()
        {
            return _documents[_random.Next(_documents.Count)];
        }

        public string GetRandomFolder()
        {
            return _folders[_random.Next(_folders.Count)];
        }

        public string GenerateRandomName()
        {
            var adjective = _adjectives[_random.Next(_adjectives.Count)];
            var noun = _nouns[_random.Next(_nouns.Count)];
            var number = _random.Next(1000);

            return $"{adjective}{noun}{number}";
        }
    }
}
