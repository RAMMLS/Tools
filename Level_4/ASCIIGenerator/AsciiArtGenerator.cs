using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiNameGenerator.Models;

namespace AsciiNameGenerator
{
    public class AsciiArtGenerator
    {
        private readonly Dictionary<string, CharacterMap> _fonts;
        private readonly Random _random;

        public AsciiArtGenerator()
        {
            _fonts = new Dictionary<string, CharacterMap>();
            _random = new Random();
            LoadFonts();
        }

        private void LoadFonts()
        {
            _fonts["Standard"] = CreateStandardFont();
            _fonts["Bold"] = CreateBoldFont();
            _fonts["Block"] = CreateBlockFont();
        }

        public string Generate(string text, GenerationOptions options)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Please provide text to generate ASCII art.";

            if (!_fonts.ContainsKey(options.FontName))
                return $"Font '{options.FontName}' not found.";

            var font = _fonts[options.FontName];
            var lines = ConvertTextToAscii(text.ToUpper(), font, options);

            return FormatOutput(lines, options);
        }

        private List<string> ConvertTextToAscii(string text, CharacterMap font, GenerationOptions options)
        {
            var outputLines = new List<string>();

            // Инициализируем линии
            for (int i = 0; i < font.Height; i++)
            {
                outputLines.Add("");
            }

            // Строим каждую букву
            foreach (char c in text)
            {
                if (font.Characters.ContainsKey(c))
                {
                    var characterLines = font.Characters[c];
                    for (int i = 0; i < font.Height; i++)
                    {
                        outputLines[i] += characterLines[i] + " ";
                    }
                }
                else
                {
                    // Если символ не найден, используем пробел
                    for (int i = 0; i < font.Height; i++)
                    {
                        outputLines[i] += new string(' ', font.Width) + " ";
                    }
                }
            }

            return outputLines;
        }

        private string FormatOutput(List<string> lines, GenerationOptions options)
        {
            var result = new StringBuilder();

            if (options.ShowBorder && lines.Count > 0 && lines[0].Length > 0)
            {
                var borderLength = Math.Max(lines[0].Length, 20);
                var border = new string('═', borderLength);
                result.AppendLine("╔" + border + "╗");
            }

            foreach (var line in lines)
            {
                if (options.ShowBorder)
                    result.Append("║ ");
                
                result.Append(line);
                
                if (options.ShowBorder)
                    result.Append(" ║");
                
                result.AppendLine();
            }

            if (options.ShowBorder && lines.Count > 0 && lines[0].Length > 0)
            {
                var borderLength = Math.Max(lines[0].Length, 20);
                var border = new string('═', borderLength);
                result.AppendLine("╚" + border + "╝");
            }

            return result.ToString().TrimEnd();
        }

        private CharacterMap CreateStandardFont()
        {
            return new CharacterMap
            {
                Name = "Standard",
                Width = 5,
                Height = 7,
                Characters = new Dictionary<char, string[]>
                {
                    ['A'] = new string[] { " ██╗", "███║", "╚██║", " ██║", " ██║", " ██║", " ╚═╝" },
                    ['B'] = new string[] { "████╗", "██╔██╗", "████╔╝", "██╔██╗", "██║╚██╗", "██║╚██╗", "╚═╝ ╚═╝" },
                    ['C'] = new string[] { " █████╗", "██╔══██╗", "██║  ╚═╝", "██║  ██╗", "╚█████╔╝", " ╚════╝", "       " },
                    ['H'] = new string[] { "██╗  ██╗", "██║  ██║", "███████║", "██╔══██║", "██║  ██║", "██║  ██║", "╚═╝  ╚═╝" },
                    ['E'] = new string[] { "███████╗", "██╔════╝", "█████╗  ", "██╔══╝  ", "███████╗", "╚══════╝", "        " },
                    ['L'] = new string[] { "██╗     ", "██║     ", "██║     ", "██║     ", "██║     ", "███████╗", "╚══════╝" },
                    ['O'] = new string[] { " ██████╗ ", "██╔═══██╗", "██║   ██║", "██║   ██║", "██║   ██║", "╚██████╔╝", " ╚═════╝ " },
                    ['R'] = new string[] { "██████╗ ", "██╔══██╗", "██████╔╝", "██╔══██╗", "██║  ██║", "██║  ██║", "╚═╝  ╚═╝" },
                    ['W'] = new string[] { "██╗    ██╗", "██║    ██║", "██║ █╗ ██║", "██║███╗██║", "╚███╔███╔╝", " ╚══╝╚══╝ ", "          " },
                    [' '] = new string[] { "   ", "   ", "   ", "   ", "   ", "   ", "   " }
                }
            };
        }

        private CharacterMap CreateBoldFont()
        {
            return new CharacterMap
            {
                Name = "Bold",
                Width = 6,
                Height = 7,
                Characters = new Dictionary<char, string[]>
                {
                    ['A'] = new string[] { " █████╗ ", "██╔══██╗", "███████║", "██╔══██║", "██║  ██║", "██║  ██║", "╚═╝  ╚═╝" },
                    ['B'] = new string[] { "██████╗ ", "██╔══██╗", "██████╔╝", "██╔══██╗", "██║  ██║", "██║  ██║", "██████╔╝" }
                }
            };
        }

        private CharacterMap CreateBlockFont()
        {
            return new CharacterMap
            {
                Name = "Block",
                Width = 7,
                Height = 7,
                Characters = new Dictionary<char, string[]>
                {
                    ['A'] = new string[] { " █████╗ ", "██╔══██╗", "██║  ██║", "███████║", "██╔══██║", "██║  ██║", "╚═╝  ╚═╝" }
                }
            };
        }

        public List<string> GetAvailableFonts()
        {
            return _fonts.Keys.ToList();
        }
    }
}
