using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinkGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string basePath = @"GeneratedLinks";
            Directory.CreateDirectory(basePath);

            var links = new List<string>
            {
                "https://example.com/file1",
                "https://example.com/file2",
                "https://example.com/file3",
                "https://example.com/file4",
                "https://example.com/file5",
                "https://example.com/archive1",
                "https://example.com/archive2",
                "https://example.com/archive3"
            };

            for (int i = 0; i < links.Count; i++)
            {
                string filePath = Path.Combine(basePath, $"link_{i + 1}.txt");
                File.WriteAllText(filePath, links[i]);
                Console.WriteLine($"Created: {filePath}");
            }

            Console.WriteLine($"\nTotal files created: {links.Count}");
            Console.WriteLine("Directory contents:");
            Directory.GetFiles(basePath)
                .ToList()
                .ForEach(f => Console.WriteLine($"  {f}"));
        }
    }
}
