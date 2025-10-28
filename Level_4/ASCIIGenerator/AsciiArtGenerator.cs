using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsciiNameGenerator.Fonts;
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
            _fonts["Standard"] = StandardFont.GetCharacterMap();
            _fonts["Bold"] = BoldFont.GetCharacterMap();
            _fonts["Block"] = BlockFont.GetCharacterMap();
        }

        public string Generate(string text, GenerationOptions options)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Text cannot be null or empty");

            if (!_fonts.ContainsKey(options.FontName))
                throw new ArgumentException($"Font '{options.FontName}' not found");

            var font = _fonts[options.FontName];
            var lines = ConvertTextToAscii(text.ToUpper(), font, options);

            return FormatOutput(lines, options);
        }

        private List<string> ConvertTextToAscii(string text, CharacterMap font, GenerationOptions options)
        {
            var outputLines = new List<string>();

            // Инициализируем линии для этого шрифта
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

            if (options.ShowBorder)
            {
                var border = new string('═', lines[0].Length + 2);
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

            if (options.ShowBorder)
            {
                var border = new string('═', lines[0].Length + 2);
                result.AppendLine("╚" + border + "╝");
            }

            return result.ToString();
        }

        public List<string> GetAvailableFonts()
        {
            return _fonts.Keys.ToList();
        }

        public string GenerateRandomArt(string text)
        {
            var availableFonts = GetAvailableFonts();
            var randomFont = availableFonts[_random.Next(availableFonts.Count)];
            
            var options = new GenerationOptions
            {
                FontName = randomFont,
                ShowBorder = _random.Next(2) == 0
            };

            return Generate(text, options);
        }
    }
}
