using System.Collections.Generic;
using AsciiNameGenerator.Models;

namespace AsciiNameGenerator.Fonts
{
    public static class BlockFont
    {
        public static CharacterMap GetCharacterMap()
        {
            return new CharacterMap
            {
                Name = "Block",
                Width = 7,
                Height = 7,
                Characters = new Dictionary<char, string[]>
                {
                    ['A'] = new string[]
                    {
                        " █████╗ ",
                        "██╔══██╗",
                        "██║  ██║",
                        "███████║",
                        "██╔══██║",
                        "██║  ██║",
                        "╚═╝  ╚═╝"
                    },
                    ['W'] = new string[]
                    {
                        "██╗    ██╗",
                        "██║    ██║",
                        "██║ █╗ ██║",
                        "██║███╗██║",
                        "╚███╔███╔╝",
                        " ╚══╝╚══╝ ",
                        "          "
                    },
                    // Добавьте другие символы...
                }
            };
        }
    }
}
