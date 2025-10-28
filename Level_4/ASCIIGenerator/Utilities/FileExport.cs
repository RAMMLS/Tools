using System;
using System.IO;
using System.Text;

namespace AsciiNameGenerator.Utilities
{
    public static class FileExport
    {
        public static void SaveToText(string asciiArt, string filename)
        {
            try
            {
                var directory = Path.GetDirectoryName(filename);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filename, asciiArt, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save file: {ex.Message}");
            }
        }

        public static void SaveToHtml(string asciiArt, string filename, string title = "ASCII Art")
        {
            try
            {
                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <title>{title}</title>
    <style>
        body {{ 
            background-color: #1a1a1a; 
            color: #00ff00; 
            font-family: 'Courier New', monospace;
            padding: 20px;
        }}
        .ascii-art {{
            white-space: pre;
            font-size: 12px;
            line-height: 1.2;
        }}
    </style>
</head>
<body>
    <div class='ascii-art'>{asciiArt}</div>
</body>
</html>";

                File.WriteAllText(filename, htmlContent, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save HTML file: {ex.Message}");
            }
        }

        public static string LoadFromFile(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    return File.ReadAllText(filename);
                }
                throw new FileNotFoundException($"File not found: {filename}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load file: {ex.Message}");
            }
        }
    }
}
