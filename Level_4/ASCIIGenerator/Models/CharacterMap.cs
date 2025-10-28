using System.Collections.Generic;

namespace AsciiNameGenerator.Models
{
    public class CharacterMap
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Dictionary<char, string[]> Characters { get; set; }

        public CharacterMap()
        {
            Characters = new Dictionary<char, string[]>();
        }
    }
}

