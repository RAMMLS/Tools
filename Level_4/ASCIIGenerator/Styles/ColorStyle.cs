namespace AsciiNameGenerator.Styles
{
    public enum ColorStyle
    {
        Default,
        Red,
        Green,
        Blue,
        Rainbow,
        Gradient
    }

    public static class AnimationStyle
    {
        public static string ApplyBlink(string text)
        {
            // Простая анимация мигания
            return $"\u001b[5m{text}\u001b[0m";
        }

        public static string ApplyWave(string text, int offset)
        {
            // Волнообразная анимация (упрощенная)
            return text;
        }
    }
}
