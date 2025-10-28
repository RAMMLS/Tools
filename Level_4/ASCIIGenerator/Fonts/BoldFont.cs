using System.Collections.Generic;
using AsciiNameGenerator.Models;

namespace AsciiNameGenerator.Fonts
{
    public static class BoldFont
    {
        public static CharacterMap GetCharacterMap()
        {
            return new CharacterMap
            {
                Name = "Bold",
                Width = 6,
                Height = 7,
                Characters = new Dictionary<char, string[]>
                {
                    ['A'] = new string[]
                    {
                        " █████╗ ",
                        "██╔══██╗",
                        "███████║",
                        "██╔══██║",
                        "██║  ██║",
                        "██║  ██║",
                        "╚═╝  ╚═╝"
                    },
                    ['B'] = new string[]
                    {
                        "██████╗ ",
                        "██╔══██╗",
                        "██████╔╝",
                        "██╔══██╗",
                        "██║  ██║",
                        "██║  ██║",
                        "██████╔╝"
                    },
                    ['H'] = new string[]
                    {
                        "██╗  ██╗",
                        "██║  ██║",
                        "███████║",
                        "██╔══██║",
                        "██║  ██║",
                        "██║  ██║",
                        "╚═╝  ╚═╝"
                    },
                    // Добавьте другие символы...
                }
            };
        }
    }
}
