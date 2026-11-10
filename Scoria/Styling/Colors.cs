namespace Scoria.Styling;

/// <summary>
/// Represents a 24-bit RGB color with red, green, and blue channels.
/// </summary>
public readonly struct Color(byte red, byte green, byte blue) : IEquatable<Color>
{
    /// <summary>Gets the red channel value.</summary>
    public byte R { get; } = red;
    /// <summary>Gets the green channel value.</summary>
    public byte G { get; } = green;
    /// <summary>Gets the blue channel value.</summary>
    public byte B { get; } = blue;

    /// <summary>Initializes a new <see cref="Color"/> with equal red, green, and blue channels.</summary>
    /// <param name="gray">The value applied to all three channels.</param>
    public Color(byte gray) : this(gray, gray, gray)
    {
    }

    /// <summary>Blends this color over another color using the given opacity.</summary>
    /// <param name="underneath">The color to blend over.</param>
    /// <param name="alpha">The opacity (0-255) of this color; the remainder is taken from <paramref name="underneath"/>.</param>
    /// <returns>The resulting blended color.</returns>
    public Color Blend(Color underneath, byte alpha)
    {
        byte alphaThis = alpha;
        byte alphaOther = (byte)(Style.MaxAlpha - alpha);
        return new Color(
                (byte)((R * alphaThis + underneath.R * alphaOther + Style.HalfAlpha) / Style.MaxAlpha),
                (byte)((G * alphaThis + underneath.G * alphaOther + Style.HalfAlpha) / Style.MaxAlpha),
                (byte)((B * alphaThis + underneath.B * alphaOther + Style.HalfAlpha) / Style.MaxAlpha)
                );
    }

    /// <summary>Adds the corresponding channels of two colors together.</summary>
    public static Color operator +(Color left, Color right)
    {
        return new((byte)(left.R + right.R), (byte)(left.G + right.G), (byte)(left.B + right.B));
    }

    /// <summary>Subtracts the corresponding channels of the right color from the left color.</summary>
    public static Color operator -(Color left, Color right)
    {
        return new((byte)(left.R - right.R), (byte)(left.G - right.G), (byte)(left.B - right.B));

    }

    /// <summary>Scales each channel of a color by a factor.</summary>
    public static Color operator *(Color color, float factor)
    {
        return new((byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
    }

    /// <summary>Scales each channel of a color by a factor.</summary>
    public static Color operator *(float factor, Color color)
    {
        return color * factor;
    }

    /// <summary>Determines whether two colors are equal.</summary>
    public static bool operator ==(Color left, Color right)
    {
        return left.R == right.R && left.G == right.G && left.B == right.B;
    }

    /// <summary>Determines whether two colors are not equal.</summary>
    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }

    /// <summary>Indicates whether the current color is equal to another color.</summary>
    /// <param name="other">A color to compare with this color.</param>
    /// <returns><see langword="true"/> if the current color is equal to the <paramref name="other"/> parameter; otherwise <see langword="false"/>.</returns>
    public readonly bool Equals(Color other)
    {
        return this == other;
    }

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Color"/> and equal to this instance; otherwise <see langword="false"/>.</returns>
    public readonly override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    /// <summary>Returns the hash code for this color.</summary>
    /// <returns>A 32-bit signed integer hash code built from the three color channels.</returns>
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }

    /// Gets a system-defined color that has an RGB value of #F0F8FF.
    public static Color AliceBlue => new(0xF0, 0xF8, 0xFF);

    /// Gets a system-defined color that has an RGB value of #FAEBD7.
    public static Color AntiqueWhite => new(0xFA, 0xEB, 0xD7);

    /// Gets a system-defined color that has an RGB value of #00FFFF.
    public static Color Aqua => new(0x00, 0xFF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #7FFFD4.
    public static Color Aquamarine => new(0x7F, 0xFF, 0xD4);

    /// Gets a system-defined color that has an RGB value of #F0FFFF.
    public static Color Azure => new(0xF0, 0xFF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #F5F5DC.
    public static Color Beige => new(0xF5, 0xF5, 0xDC);

    /// Gets a system-defined color that has an RGB value of #FFE4C4.
    public static Color Bisque => new(0xFF, 0xE4, 0xC4);

    /// Gets a system-defined color that has an RGB value of #000000.
    public static Color Black => new(0x00, 0x00, 0x00);

    /// Gets a system-defined color that has an RGB value of #FFEBCD.
    public static Color BlanchedAlmond => new(0xFF, 0xEB, 0xCD);

    /// Gets a system-defined color that has an RGB value of #0000FF.
    public static Color Blue => new(0x00, 0x00, 0xFF);

    /// Gets a system-defined color that has an RGB value of #8A2BE2.
    public static Color BlueViolet => new(0x8A, 0x2B, 0xE2);

    /// Gets a system-defined color that has an RGB value of #A52A2A.
    public static Color Brown => new(0xA5, 0x2A, 0x2A);

    /// Gets a system-defined color that has an RGB value of #DEB887.
    public static Color BurlyWood => new(0xDE, 0xB8, 0x87);

    /// Gets a system-defined color that has an RGB value of #5F9EA0.
    public static Color CadetBlue => new(0x5F, 0x9E, 0xA0);

    /// Gets a system-defined color that has an RGB value of #7FFF00.
    public static Color Chartreuse => new(0x7F, 0xFF, 0x00);

    /// Gets a system-defined color that has an RGB value of #D2691E.
    public static Color Chocolate => new(0xD2, 0x69, 0x1E);

    /// Gets a system-defined color that has an RGB value of #FF7F50.
    public static Color Coral => new(0xFF, 0x7F, 0x50);

    /// Gets a system-defined color that has an RGB value of #6495ED.
    public static Color CornflowerBlue => new(0x64, 0x95, 0xED);

    /// Gets a system-defined color that has an RGB value of #FFF8DC.
    public static Color Cornsilk => new(0xFF, 0xF8, 0xDC);

    /// Gets a system-defined color that has an RGB value of #DC143C.
    public static Color Crimson => new(0xDC, 0x14, 0x3C);

    /// Gets a system-defined color that has an RGB value of #00FFFF.
    public static Color Cyan => new(0x00, 0xFF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #00008B.
    public static Color DarkBlue => new(0x00, 0x00, 0x8B);

    /// Gets a system-defined color that has an RGB value of #008B8B.
    public static Color DarkCyan => new(0x00, 0x8B, 0x8B);

    /// Gets a system-defined color that has an RGB value of #B8860B.
    public static Color DarkGoldenrod => new(0xB8, 0x86, 0x0B);

    /// Gets a system-defined color that has an RGB value of #A9A9A9.
    public static Color DarkGray => new(0xA9, 0xA9, 0xA9);

    /// Gets a system-defined color that has an RGB value of #006400.
    public static Color DarkGreen => new(0x00, 0x64, 0x00);

    /// Gets a system-defined color that has an RGB value of #BDB76B.
    public static Color DarkKhaki => new(0xBD, 0xB7, 0x6B);

    /// Gets a system-defined color that has an RGB value of #8B008B.
    public static Color DarkMagenta => new(0x8B, 0x00, 0x8B);

    /// Gets a system-defined color that has an RGB value of #556B2F.
    public static Color DarkOliveGreen => new(0x55, 0x6B, 0x2F);

    /// Gets a system-defined color that has an RGB value of #FF8C00.
    public static Color DarkOrange => new(0xFF, 0x8C, 0x00);

    /// Gets a system-defined color that has an RGB value of #9932CC.
    public static Color DarkOrchid => new(0x99, 0x32, 0xCC);

    /// Gets a system-defined color that has an RGB value of #8B0000.
    public static Color DarkRed => new(0x8B, 0x00, 0x00);

    /// Gets a system-defined color that has an RGB value of #E9967A.
    public static Color DarkSalmon => new(0xE9, 0x96, 0x7A);

    /// Gets a system-defined color that has an RGB value of #8FBC8F.
    public static Color DarkSeaGreen => new(0x8F, 0xBC, 0x8F);

    /// Gets a system-defined color that has an RGB value of #483D8B.
    public static Color DarkSlateBlue => new(0x48, 0x3D, 0x8B);

    /// Gets a system-defined color that has an RGB value of #2F4F4F.
    public static Color DarkSlateGray => new(0x2F, 0x4F, 0x4F);

    /// Gets a system-defined color that has an RGB value of #00CED1.
    public static Color DarkTurquoise => new(0x00, 0xCE, 0xD1);

    /// Gets a system-defined color that has an RGB value of #9400D3.
    public static Color DarkViolet => new(0x94, 0x00, 0xD3);

    /// Gets a system-defined color that has an RGB value of #FF1493.
    public static Color DeepPink => new(0xFF, 0x14, 0x93);

    /// Gets a system-defined color that has an RGB value of #00BFFF.
    public static Color DeepSkyBlue => new(0x00, 0xBF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #696969.
    public static Color DimGray => new(0x69, 0x69, 0x69);

    /// Gets a system-defined color that has an RGB value of #1E90FF.
    public static Color DodgerBlue => new(0x1E, 0x90, 0xFF);

    /// Gets a system-defined color that has an RGB value of #B22222.
    public static Color Firebrick => new(0xB2, 0x22, 0x22);

    /// Gets a system-defined color that has an RGB value of #FFFAF0.
    public static Color FloralWhite => new(0xFF, 0xFA, 0xF0);

    /// Gets a system-defined color that has an RGB value of #228B22.
    public static Color ForestGreen => new(0x22, 0x8B, 0x22);

    /// Gets a system-defined color that has an RGB value of #FF00FF.
    public static Color Fuchsia => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an RGB value of #DCDCDC.
    public static Color Gainsboro => new(0xDC, 0xDC, 0xDC);

    /// Gets a system-defined color that has an RGB value of #F8F8FF.
    public static Color GhostWhite => new(0xF8, 0xF8, 0xFF);

    /// Gets a system-defined color that has an RGB value of #FFD700.
    public static Color Gold => new(0xFF, 0xD7, 0x00);

    /// Gets a system-defined color that has an RGB value of #DAA520.
    public static Color Goldenrod => new(0xDA, 0xA5, 0x20);

    /// Gets a system-defined color that has an RGB value of #808080.
    public static Color Gray => new(0x80, 0x80, 0x80);

    /// Gets a system-defined color that has an RGB value of #008000.
    public static Color Green => new(0x00, 0x80, 0x00);

    /// Gets a system-defined color that has an RGB value of #ADFF2F.
    public static Color GreenYellow => new(0xAD, 0xFF, 0x2F);

    /// Gets a system-defined color that has an RGB value of #F0FFF0.
    public static Color Honeydew => new(0xF0, 0xFF, 0xF0);

    /// Gets a system-defined color that has an RGB value of #FF69B4.
    public static Color HotPink => new(0xFF, 0x69, 0xB4);

    /// Gets a system-defined color that has an RGB value of #CD5C5C.
    public static Color IndianRed => new(0xCD, 0x5C, 0x5C);

    /// Gets a system-defined color that has an RGB value of #4B0082.
    public static Color Indigo => new(0x4B, 0x00, 0x82);

    /// Gets a system-defined color that has an RGB value of #FFFFF0.
    public static Color Ivory => new(0xFF, 0xFF, 0xF0);

    /// Gets a system-defined color that has an RGB value of #F0E68C.
    public static Color Khaki => new(0xF0, 0xE6, 0x8C);

    /// Gets a system-defined color that has an RGB value of #E6E6FA.
    public static Color Lavender => new(0xE6, 0xE6, 0xFA);

    /// Gets a system-defined color that has an RGB value of #FFF0F5.
    public static Color LavenderBlush => new(0xFF, 0xF0, 0xF5);

    /// Gets a system-defined color that has an RGB value of #7CFC00.
    public static Color LawnGreen => new(0x7C, 0xFC, 0x00);

    /// Gets a system-defined color that has an RGB value of #FFFACD.
    public static Color LemonChiffon => new(0xFF, 0xFA, 0xCD);

    /// Gets a system-defined color that has an RGB value of #ADD8E6.
    public static Color LightBlue => new(0xAD, 0xD8, 0xE6);

    /// Gets a system-defined color that has an RGB value of #F08080.
    public static Color LightCoral => new(0xF0, 0x80, 0x80);

    /// Gets a system-defined color that has an RGB value of #E0FFFF.
    public static Color LightCyan => new(0xE0, 0xFF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #FAFAD2.
    public static Color LightGoldenrodYellow => new(0xFA, 0xFA, 0xD2);

    /// Gets a system-defined color that has an RGB value of #D3D3D3.
    public static Color LightGray => new(0xD3, 0xD3, 0xD3);

    /// Gets a system-defined color that has an RGB value of #90EE90.
    public static Color LightGreen => new(0x90, 0xEE, 0x90);

    /// Gets a system-defined color that has an RGB value of #FFB6C1.
    public static Color LightPink => new(0xFF, 0xB6, 0xC1);

    /// Gets a system-defined color that has an RGB value of #FFA07A.
    public static Color LightSalmon => new(0xFF, 0xA0, 0x7A);

    /// Gets a system-defined color that has an RGB value of #20B2AA.
    public static Color LightSeaGreen => new(0x20, 0xB2, 0xAA);

    /// Gets a system-defined color that has an RGB value of #87CEFA.
    public static Color LightSkyBlue => new(0x87, 0xCE, 0xFA);

    /// Gets a system-defined color that has an RGB value of #778899.
    public static Color LightSlateGray => new(0x77, 0x88, 0x99);

    /// Gets a system-defined color that has an RGB value of #B0C4DE.
    public static Color LightSteelBlue => new(0xB0, 0xC4, 0xDE);

    /// Gets a system-defined color that has an RGB value of #FFFFE0.
    public static Color LightYellow => new(0xFF, 0xFF, 0xE0);

    /// Gets a system-defined color that has an RGB value of #00FF00.
    public static Color Lime => new(0x00, 0xFF, 0x00);

    /// Gets a system-defined color that has an RGB value of #32CD32.
    public static Color LimeGreen => new(0x32, 0xCD, 0x32);

    /// Gets a system-defined color that has an RGB value of #FAF0E6.
    public static Color Linen => new(0xFA, 0xF0, 0xE6);

    /// Gets a system-defined color that has an RGB value of #FF00FF.
    public static Color Magenta => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an RGB value of #800000.
    public static Color Maroon => new(0x80, 0x00, 0x00);

    /// Gets a system-defined color that has an RGB value of #66CDAA.
    public static Color MediumAquamarine => new(0x66, 0xCD, 0xAA);

    /// Gets a system-defined color that has an RGB value of #0000CD.
    public static Color MediumBlue => new(0x00, 0x00, 0xCD);

    /// Gets a system-defined color that has an RGB value of #BA55D3.
    public static Color MediumOrchid => new(0xBA, 0x55, 0xD3);

    /// Gets a system-defined color that has an RGB value of #9370DB.
    public static Color MediumPurple => new(0x93, 0x70, 0xDB);

    /// Gets a system-defined color that has an RGB value of #3CB371.
    public static Color MediumSeaGreen => new(0x3C, 0xB3, 0x71);

    /// Gets a system-defined color that has an RGB value of #7B68EE.
    public static Color MediumSlateBlue => new(0x7B, 0x68, 0xEE);

    /// Gets a system-defined color that has an RGB value of #00FA9A.
    public static Color MediumSpringGreen => new(0x00, 0xFA, 0x9A);

    /// Gets a system-defined color that has an RGB value of #48D1CC.
    public static Color MediumTurquoise => new(0x48, 0xD1, 0xCC);

    /// Gets a system-defined color that has an RGB value of #C71585.
    public static Color MediumVioletRed => new(0xC7, 0x15, 0x85);

    /// Gets a system-defined color that has an RGB value of #191970.
    public static Color MidnightBlue => new(0x19, 0x19, 0x70);

    /// Gets a system-defined color that has an RGB value of #F5FFFA.
    public static Color MintCream => new(0xF5, 0xFF, 0xFA);

    /// Gets a system-defined color that has an RGB value of #FFE4E1.
    public static Color MistyRose => new(0xFF, 0xE4, 0xE1);

    /// Gets a system-defined color that has an RGB value of #FFE4B5.
    public static Color Moccasin => new(0xFF, 0xE4, 0xB5);

    /// Gets a system-defined color that has an RGB value of #FFDEAD.
    public static Color NavajoWhite => new(0xFF, 0xDE, 0xAD);

    /// Gets a system-defined color that has an RGB value of #000080.
    public static Color Navy => new(0x00, 0x00, 0x80);

    /// Gets a system-defined color that has an RGB value of #FDF5E6.
    public static Color OldLace => new(0xFD, 0xF5, 0xE6);

    /// Gets a system-defined color that has an RGB value of #808000.
    public static Color Olive => new(0x80, 0x80, 0x00);

    /// Gets a system-defined color that has an RGB value of #6B8E23.
    public static Color OliveDrab => new(0x6B, 0x8E, 0x23);

    /// Gets a system-defined color that has an RGB value of #FFA500.
    public static Color Orange => new(0xFF, 0xA5, 0x00);

    /// Gets a system-defined color that has an RGB value of #FF4500.
    public static Color OrangeRed => new(0xFF, 0x45, 0x00);

    /// Gets a system-defined color that has an RGB value of #DA70D6.
    public static Color Orchid => new(0xDA, 0x70, 0xD6);

    /// Gets a system-defined color that has an RGB value of #EEE8AA.
    public static Color PaleGoldenrod => new(0xEE, 0xE8, 0xAA);

    /// Gets a system-defined color that has an RGB value of #98FB98.
    public static Color PaleGreen => new(0x98, 0xFB, 0x98);

    /// Gets a system-defined color that has an RGB value of #AFEEEE.
    public static Color PaleTurquoise => new(0xAF, 0xEE, 0xEE);

    /// Gets a system-defined color that has an RGB value of #DB7093.
    public static Color PaleVioletRed => new(0xDB, 0x70, 0x93);

    /// Gets a system-defined color that has an RGB value of #FFEFD5.
    public static Color PapayaWhip => new(0xFF, 0xEF, 0xD5);

    /// Gets a system-defined color that has an RGB value of #FFDAB9.
    public static Color PeachPuff => new(0xFF, 0xDA, 0xB9);

    /// Gets a system-defined color that has an RGB value of #CD853F.
    public static Color Peru => new(0xCD, 0x85, 0x3F);

    /// Gets a system-defined color that has an RGB value of #FFC0CB.
    public static Color Pink => new(0xFF, 0xC0, 0xCB);

    /// Gets a system-defined color that has an RGB value of #DDA0DD.
    public static Color Plum => new(0xDD, 0xA0, 0xDD);

    /// Gets a system-defined color that has an RGB value of #B0E0E6.
    public static Color PowderBlue => new(0xB0, 0xE0, 0xE6);

    /// Gets a system-defined color that has an RGB value of #800080.
    public static Color Purple => new(0x80, 0x00, 0x80);

    /// Gets a system-defined color that has an RGB value of #663399.
    public static Color RebeccaPurple => new(0x66, 0x33, 0x99);

    /// Gets a system-defined color that has an RGB value of #FF0000.
    public static Color Red => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an RGB value of #BC8F8F.
    public static Color RosyBrown => new(0xBC, 0x8F, 0x8F);

    /// Gets a system-defined color that has an RGB value of #4169E1.
    public static Color RoyalBlue => new(0x41, 0x69, 0xE1);

    /// Gets a system-defined color that has an RGB value of #8B4513.
    public static Color SaddleBrown => new(0x8B, 0x45, 0x13);

    /// Gets a system-defined color that has an RGB value of #FA8072.
    public static Color Salmon => new(0xFA, 0x80, 0x72);

    /// Gets a system-defined color that has an RGB value of #F4A460.
    public static Color SandyBrown => new(0xF4, 0xA4, 0x60);

    /// Gets a system-defined color that has an RGB value of #2E8B57.
    public static Color SeaGreen => new(0x2E, 0x8B, 0x57);

    /// Gets a system-defined color that has an RGB value of #FFF5EE.
    public static Color SeaShell => new(0xFF, 0xF5, 0xEE);

    /// Gets a system-defined color that has an RGB value of #A0522D.
    public static Color Sienna => new(0xA0, 0x52, 0x2D);

    /// Gets a system-defined color that has an RGB value of #C0C0C0.
    public static Color Silver => new(0xC0, 0xC0, 0xC0);

    /// Gets a system-defined color that has an RGB value of #87CEEB.
    public static Color SkyBlue => new(0x87, 0xCE, 0xEB);

    /// Gets a system-defined color that has an RGB value of #6A5ACD.
    public static Color SlateBlue => new(0x6A, 0x5A, 0xCD);

    /// Gets a system-defined color that has an RGB value of #708090.
    public static Color SlateGray => new(0x70, 0x80, 0x90);

    /// Gets a system-defined color that has an RGB value of #FFFAFA.
    public static Color Snow => new(0xFF, 0xFA, 0xFA);

    /// Gets a system-defined color that has an RGB value of #00FF7F.
    public static Color SpringGreen => new(0x00, 0xFF, 0x7F);

    /// Gets a system-defined color that has an RGB value of #4682B4.
    public static Color SteelBlue => new(0x46, 0x82, 0xB4);

    /// Gets a system-defined color that has an RGB value of #D2B48C.
    public static Color Tan => new(0xD2, 0xB4, 0x8C);

    /// Gets a system-defined color that has an RGB value of #008080.
    public static Color Teal => new(0x00, 0x80, 0x80);

    /// Gets a system-defined color that has an RGB value of #D8BFD8.
    public static Color Thistle => new(0xD8, 0xBF, 0xD8);

    /// Gets a system-defined color that has an RGB value of #FF6347.
    public static Color Tomato => new(0xFF, 0x63, 0x47);

    /// Gets a system-defined color that has an RGB value of #40E0D0.
    public static Color Turquoise => new(0x40, 0xE0, 0xD0);

    /// Gets a system-defined color that has an RGB value of #EE82EE.
    public static Color Violet => new(0xEE, 0x82, 0xEE);

    /// Gets a system-defined color that has an RGB value of #F5DEB3.
    public static Color Wheat => new(0xF5, 0xDE, 0xB3);

    /// Gets a system-defined color that has an RGB value of #FFFFFF.
    public static Color White => new(0xFF, 0xFF, 0xFF);

    /// Gets a system-defined color that has an RGB value of #F5F5F5.
    public static Color WhiteSmoke => new(0xF5, 0xF5, 0xF5);

    /// Gets a system-defined color that has an RGB value of #FFFF00.
    public static Color Yellow => new(0xFF, 0xFF, 0x00);

    /// Gets a system-defined color that has an RGB value of #9ACD32.
    public static Color YellowGreen => new(0x9A, 0xCD, 0x32);

}

