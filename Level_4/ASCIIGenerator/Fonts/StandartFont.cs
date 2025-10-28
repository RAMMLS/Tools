using System.Collections.Generic;
using AsciiNameGenerator.Models;

namespace AsciiNameGenerator.Fonts
{
    public static class StandardFont
    {
        public static CharacterMap GetCharacterMap()
        {
            return new CharacterMap
            {
                Name = "Standard",
                Width = 5,
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
                        "╚█████╔╝"
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
                    ['C'] = new string[]
                    {
                        " ██████╗",
                        "██╔════╝",
                        "██║     ",
                        "██║     ",
                        "██║     ",
                        "██║     ",
                        "╚██████╗"
                    },
                    ['D'] = new string[]
                    {
                        "██████╗ ",
                        "██╔══██╗",
                        "██║  ██║",
                        "██║  ██║",
                        "██║  ██║",
                        "██║  ██║",
                        "██████╔╝"
                    },
                    ['E'] = new string[]
                    {
                        "███████╗",
                        "██╔════╝",
                        "█████╗  ",
                        "██╔══╝  ",
                        "█████╗  ",
                        "██╔══╝  ",
                        "███████╗"
                    },
                    // Добавьте остальные буквы алфавита...
                    [' '] = new string[]
                    {
                        "     ",
                        "     ",
                        "     ",
                        "     ",
                        "     ",
                        "     ",
                        "     "
                    }
                }
            };
        }
    }
}
