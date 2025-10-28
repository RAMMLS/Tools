using AsciiNameGenerator.Styles;

namespace AsciiNameGenerator.Models
{
    public class GenerationOptions
    {
        public string FontName { get; set; } = "Standard";
        public ColorStyle ColorStyle { get; set; } = ColorStyle.Default;
        public int Width { get; set; } = 80;
        public int Height { get; set; } = 25;
        public bool ShowBorder { get; set; } = true;
        public bool UseAnimation { get; set; } = false;
        public int AnimationSpeed { get; set; } = 100;

        public GenerationOptions()
        {
        }

        public GenerationOptions(string fontName, ColorStyle colorStyle)
        {
            FontName = fontName;
            ColorStyle = colorStyle;
        }
    }
}

