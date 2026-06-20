namespace Scoria;

[Flags]
public enum StyleAttributes : byte
{
    None = 0,
    Bold = 1 << 0,
    Italic = 1 << 1,
    Underline = 1 << 2,
    Strikethrough = 1 << 3,
    Blink = 1 << 4,
    DoubleUnderline = 1 << 5,
    Overline = 1 << 6,
}

public struct Style()
{
    public byte ForegroundRed { get; set; } = 255;
    public byte ForegroundGreen { get; set; } = 255;
    public byte ForegroundBlue { get; set; } = 255;
    public byte BackgroundRed { get; set; }
    public byte BackgroundGreen { get; set; }
    public byte BackgroundBlue { get; set; }
    public byte BackgroundAlpha { get; set; }
    public StyleAttributes StyleAttributes { get; set; }

    public Style(byte foregroundRed, byte foregroundGreen, byte foregroundBlue,
        byte backgroundRed, byte backgroundGreen, byte backgroundBlue, byte backgroundAlpha,
        StyleAttributes styleAttributes = StyleAttributes.None) : this()
    {
        ForegroundRed = foregroundRed;
        ForegroundGreen = foregroundGreen;
        ForegroundBlue = foregroundBlue;
        BackgroundRed = backgroundRed;
        BackgroundGreen = backgroundGreen;
        BackgroundBlue = backgroundBlue;
        BackgroundAlpha = backgroundAlpha;
        StyleAttributes = styleAttributes;
    }

    public Style(byte foregroundRed, byte foregroundGreen, byte foregroundBlue,
        StyleAttributes styleAttributes = StyleAttributes.None) : this()
    {
        ForegroundRed = foregroundRed;
        ForegroundGreen = foregroundGreen;
        ForegroundBlue = foregroundBlue;
        StyleAttributes = styleAttributes;
    }

    public Style(StyleAttributes styleAttributes) : this()
    {
        StyleAttributes = styleAttributes;
    }
}