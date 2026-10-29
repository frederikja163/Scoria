namespace Scoria.Styling;

public struct Color(byte red, byte green, byte blue) : IEquatable<Color>
{
    public byte R { get; set; } = red;
    public byte G { get; set; } = green;
    public byte B { get; set; } = blue;

    public Color(byte gray) : this(gray, gray, gray)
    {
    }

    public static Color operator +(Color left, Color right)
    {
        return new((byte)(left.R + right.R), (byte)(left.G + right.G), (byte)(left.B + right.B));
    }

    public static Color operator -(Color left, Color right)
    {
        return new((byte)(left.R - right.R), (byte)(left.G - right.G), (byte)(left.B - right.B));

    }

    public static Color operator *(Color color, float factor)
    {
        return new((byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
    }

    public static Color operator *(float factor, Color color)
    {
        return color * factor;
    }

    public static bool operator ==(Color left, Color right)
    {
        return left.R == right.R && left.G == right.G && left.B == right.B;
    }

    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }

    public readonly bool Equals(Color other)
    {
        return this == other;
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, G, B);
    }

    /// Gets a system-defined color that has an ARGB value of #FFF0F8.
    public static Color AliceBlue => new(0xFF, 0xF0, 0xF8);

    /// Gets a system-defined color that has an ARGB value of #FFFAEB.
    public static Color AntiqueWhite => new(0xFF, 0xFA, 0xEB);

    /// Gets a system-defined color that has an ARGB value of #FF00FF.
    public static Color Aqua => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF7FFF.
    public static Color Aquamarine => new(0xFF, 0x7F, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFF0FF.
    public static Color Azure => new(0xFF, 0xF0, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFF5F5.
    public static Color Beige => new(0xFF, 0xF5, 0xF5);

    /// Gets a system-defined color that has an ARGB value of #FFFFE4.
    public static Color Bisque => new(0xFF, 0xFF, 0xE4);

    /// Gets a system-defined color that has an ARGB value of #FF0000.
    public static Color Black => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFFFEB.
    public static Color BlanchedAlmond => new(0xFF, 0xFF, 0xEB);

    /// Gets a system-defined color that has an ARGB value of #FF0000.
    public static Color Blue => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FF8A2B.
    public static Color BlueViolet => new(0xFF, 0x8A, 0x2B);

    /// Gets a system-defined color that has an ARGB value of #FFA52A.
    public static Color Brown => new(0xFF, 0xA5, 0x2A);

    /// Gets a system-defined color that has an ARGB value of #FFDEB8.
    public static Color BurlyWood => new(0xFF, 0xDE, 0xB8);

    /// Gets a system-defined color that has an ARGB value of #FF5F9E.
    public static Color CadetBlue => new(0xFF, 0x5F, 0x9E);

    /// Gets a system-defined color that has an ARGB value of #FF7FFF.
    public static Color Chartreuse => new(0xFF, 0x7F, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFD269.
    public static Color Chocolate => new(0xFF, 0xD2, 0x69);

    /// Gets a system-defined color that has an ARGB value of #FFFF7F.
    public static Color Coral => new(0xFF, 0xFF, 0x7F);

    /// Gets a system-defined color that has an ARGB value of #FF6495.
    public static Color CornflowerBlue => new(0xFF, 0x64, 0x95);

    /// Gets a system-defined color that has an ARGB value of #FFFFF8.
    public static Color Cornsilk => new(0xFF, 0xFF, 0xF8);

    /// Gets a system-defined color that has an ARGB value of #FFDC14.
    public static Color Crimson => new(0xFF, 0xDC, 0x14);

    /// Gets a system-defined color that has an ARGB value of #FF00FF.
    public static Color Cyan => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF0000.
    public static Color DarkBlue => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FF008B.
    public static Color DarkCyan => new(0xFF, 0x00, 0x8B);

    /// Gets a system-defined color that has an ARGB value of #FFB886.
    public static Color DarkGoldenrod => new(0xFF, 0xB8, 0x86);

    /// Gets a system-defined color that has an ARGB value of #FFA9A9.
    public static Color DarkGray => new(0xFF, 0xA9, 0xA9);

    /// Gets a system-defined color that has an ARGB value of #FF0064.
    public static Color DarkGreen => new(0xFF, 0x00, 0x64);

    /// Gets a system-defined color that has an ARGB value of #FFBDB7.
    public static Color DarkKhaki => new(0xFF, 0xBD, 0xB7);

    /// Gets a system-defined color that has an ARGB value of #FF8B00.
    public static Color DarkMagenta => new(0xFF, 0x8B, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FF556B.
    public static Color DarkOliveGreen => new(0xFF, 0x55, 0x6B);

    /// Gets a system-defined color that has an ARGB value of #FFFF8C.
    public static Color DarkOrange => new(0xFF, 0xFF, 0x8C);

    /// Gets a system-defined color that has an ARGB value of #FF9932.
    public static Color DarkOrchid => new(0xFF, 0x99, 0x32);

    /// Gets a system-defined color that has an ARGB value of #FF8B00.
    public static Color DarkRed => new(0xFF, 0x8B, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFE996.
    public static Color DarkSalmon => new(0xFF, 0xE9, 0x96);

    /// Gets a system-defined color that has an ARGB value of #FF8FBC.
    public static Color DarkSeaGreen => new(0xFF, 0x8F, 0xBC);

    /// Gets a system-defined color that has an ARGB value of #FF483D.
    public static Color DarkSlateBlue => new(0xFF, 0x48, 0x3D);

    /// Gets a system-defined color that has an ARGB value of #FF2F4F.
    public static Color DarkSlateGray => new(0xFF, 0x2F, 0x4F);

    /// Gets a system-defined color that has an ARGB value of #FF00CE.
    public static Color DarkTurquoise => new(0xFF, 0x00, 0xCE);

    /// Gets a system-defined color that has an ARGB value of #FF9400.
    public static Color DarkViolet => new(0xFF, 0x94, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFFF14.
    public static Color DeepPink => new(0xFF, 0xFF, 0x14);

    /// Gets a system-defined color that has an ARGB value of #FF00BF.
    public static Color DeepSkyBlue => new(0xFF, 0x00, 0xBF);

    /// Gets a system-defined color that has an ARGB value of #FF6969.
    public static Color DimGray => new(0xFF, 0x69, 0x69);

    /// Gets a system-defined color that has an ARGB value of #FF1E90.
    public static Color DodgerBlue => new(0xFF, 0x1E, 0x90);

    /// Gets a system-defined color that has an ARGB value of #FFB222.
    public static Color Firebrick => new(0xFF, 0xB2, 0x22);

    /// Gets a system-defined color that has an ARGB value of #FFFFFA.
    public static Color FloralWhite => new(0xFF, 0xFF, 0xFA);

    /// Gets a system-defined color that has an ARGB value of #FF228B.
    public static Color ForestGreen => new(0xFF, 0x22, 0x8B);

    /// Gets a system-defined color that has an ARGB value of #FFFF00.
    public static Color Fuchsia => new(0xFF, 0xFF, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFDCDC.
    public static Color Gainsboro => new(0xFF, 0xDC, 0xDC);

    /// Gets a system-defined color that has an ARGB value of #FFF8F8.
    public static Color GhostWhite => new(0xFF, 0xF8, 0xF8);

    /// Gets a system-defined color that has an ARGB value of #FFFFD7.
    public static Color Gold => new(0xFF, 0xFF, 0xD7);

    /// Gets a system-defined color that has an ARGB value of #FFDAA5.
    public static Color Goldenrod => new(0xFF, 0xDA, 0xA5);

    /// Gets a system-defined color that has an ARGB value of #FF8080.
    public static Color Gray => new(0xFF, 0x80, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FF0080.
    public static Color Green => new(0xFF, 0x00, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FFADFF.
    public static Color GreenYellow => new(0xFF, 0xAD, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFF0FF.
    public static Color Honeydew => new(0xFF, 0xF0, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFFF69.
    public static Color HotPink => new(0xFF, 0xFF, 0x69);

    /// Gets a system-defined color that has an ARGB value of #FFCD5C.
    public static Color IndianRed => new(0xFF, 0xCD, 0x5C);

    /// Gets a system-defined color that has an ARGB value of #FF4B00.
    public static Color Indigo => new(0xFF, 0x4B, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFFFFF.
    public static Color Ivory => new(0xFF, 0xFF, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFF0E6.
    public static Color Khaki => new(0xFF, 0xF0, 0xE6);

    /// Gets a system-defined color that has an ARGB value of #FFE6E6.
    public static Color Lavender => new(0xFF, 0xE6, 0xE6);

    /// Gets a system-defined color that has an ARGB value of #FFFFF0.
    public static Color LavenderBlush => new(0xFF, 0xFF, 0xF0);

    /// Gets a system-defined color that has an ARGB value of #FF7CFC.
    public static Color LawnGreen => new(0xFF, 0x7C, 0xFC);

    /// Gets a system-defined color that has an ARGB value of #FFFFFA.
    public static Color LemonChiffon => new(0xFF, 0xFF, 0xFA);

    /// Gets a system-defined color that has an ARGB value of #FFADD8.
    public static Color LightBlue => new(0xFF, 0xAD, 0xD8);

    /// Gets a system-defined color that has an ARGB value of #FFF080.
    public static Color LightCoral => new(0xFF, 0xF0, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FFE0FF.
    public static Color LightCyan => new(0xFF, 0xE0, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFFAFA.
    public static Color LightGoldenrodYellow => new(0xFF, 0xFA, 0xFA);

    /// Gets a system-defined color that has an ARGB value of #FFD3D3.
    public static Color LightGray => new(0xFF, 0xD3, 0xD3);

    /// Gets a system-defined color that has an ARGB value of #FF90EE.
    public static Color LightGreen => new(0xFF, 0x90, 0xEE);

    /// Gets a system-defined color that has an ARGB value of #FFFFB6.
    public static Color LightPink => new(0xFF, 0xFF, 0xB6);

    /// Gets a system-defined color that has an ARGB value of #FFFFA0.
    public static Color LightSalmon => new(0xFF, 0xFF, 0xA0);

    /// Gets a system-defined color that has an ARGB value of #FF20B2.
    public static Color LightSeaGreen => new(0xFF, 0x20, 0xB2);

    /// Gets a system-defined color that has an ARGB value of #FF87CE.
    public static Color LightSkyBlue => new(0xFF, 0x87, 0xCE);

    /// Gets a system-defined color that has an ARGB value of #FF7788.
    public static Color LightSlateGray => new(0xFF, 0x77, 0x88);

    /// Gets a system-defined color that has an ARGB value of #FFB0C4.
    public static Color LightSteelBlue => new(0xFF, 0xB0, 0xC4);

    /// Gets a system-defined color that has an ARGB value of #FFFFFF.
    public static Color LightYellow => new(0xFF, 0xFF, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF00FF.
    public static Color Lime => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF32CD.
    public static Color LimeGreen => new(0xFF, 0x32, 0xCD);

    /// Gets a system-defined color that has an ARGB value of #FFFAF0.
    public static Color Linen => new(0xFF, 0xFA, 0xF0);

    /// Gets a system-defined color that has an ARGB value of #FFFF00.
    public static Color Magenta => new(0xFF, 0xFF, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FF8000.
    public static Color Maroon => new(0xFF, 0x80, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FF66CD.
    public static Color MediumAquamarine => new(0xFF, 0x66, 0xCD);

    /// Gets a system-defined color that has an ARGB value of #FF0000.
    public static Color MediumBlue => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFBA55.
    public static Color MediumOrchid => new(0xFF, 0xBA, 0x55);

    /// Gets a system-defined color that has an ARGB value of #FF9370.
    public static Color MediumPurple => new(0xFF, 0x93, 0x70);

    /// Gets a system-defined color that has an ARGB value of #FF3CB3.
    public static Color MediumSeaGreen => new(0xFF, 0x3C, 0xB3);

    /// Gets a system-defined color that has an ARGB value of #FF7B68.
    public static Color MediumSlateBlue => new(0xFF, 0x7B, 0x68);

    /// Gets a system-defined color that has an ARGB value of #FF00FA.
    public static Color MediumSpringGreen => new(0xFF, 0x00, 0xFA);

    /// Gets a system-defined color that has an ARGB value of #FF48D1.
    public static Color MediumTurquoise => new(0xFF, 0x48, 0xD1);

    /// Gets a system-defined color that has an ARGB value of #FFC715.
    public static Color MediumVioletRed => new(0xFF, 0xC7, 0x15);

    /// Gets a system-defined color that has an ARGB value of #FF1919.
    public static Color MidnightBlue => new(0xFF, 0x19, 0x19);

    /// Gets a system-defined color that has an ARGB value of #FFF5FF.
    public static Color MintCream => new(0xFF, 0xF5, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFFFE4.
    public static Color MistyRose => new(0xFF, 0xFF, 0xE4);

    /// Gets a system-defined color that has an ARGB value of #FFFFE4.
    public static Color Moccasin => new(0xFF, 0xFF, 0xE4);

    /// Gets a system-defined color that has an ARGB value of #FFFFDE.
    public static Color NavajoWhite => new(0xFF, 0xFF, 0xDE);

    /// Gets a system-defined color that has an ARGB value of #FF0000.
    public static Color Navy => new(0xFF, 0x00, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFFDF5.
    public static Color OldLace => new(0xFF, 0xFD, 0xF5);

    /// Gets a system-defined color that has an ARGB value of #FF8080.
    public static Color Olive => new(0xFF, 0x80, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FF6B8E.
    public static Color OliveDrab => new(0xFF, 0x6B, 0x8E);

    /// Gets a system-defined color that has an ARGB value of #FFFFA5.
    public static Color Orange => new(0xFF, 0xFF, 0xA5);

    /// Gets a system-defined color that has an ARGB value of #FFFF45.
    public static Color OrangeRed => new(0xFF, 0xFF, 0x45);

    /// Gets a system-defined color that has an ARGB value of #FFDA70.
    public static Color Orchid => new(0xFF, 0xDA, 0x70);

    /// Gets a system-defined color that has an ARGB value of #FFEEE8.
    public static Color PaleGoldenrod => new(0xFF, 0xEE, 0xE8);

    /// Gets a system-defined color that has an ARGB value of #FF98FB.
    public static Color PaleGreen => new(0xFF, 0x98, 0xFB);

    /// Gets a system-defined color that has an ARGB value of #FFAFEE.
    public static Color PaleTurquoise => new(0xFF, 0xAF, 0xEE);

    /// Gets a system-defined color that has an ARGB value of #FFDB70.
    public static Color PaleVioletRed => new(0xFF, 0xDB, 0x70);

    /// Gets a system-defined color that has an ARGB value of #FFFFEF.
    public static Color PapayaWhip => new(0xFF, 0xFF, 0xEF);

    /// Gets a system-defined color that has an ARGB value of #FFFFDA.
    public static Color PeachPuff => new(0xFF, 0xFF, 0xDA);

    /// Gets a system-defined color that has an ARGB value of #FFCD85.
    public static Color Peru => new(0xFF, 0xCD, 0x85);

    /// Gets a system-defined color that has an ARGB value of #FFFFC0.
    public static Color Pink => new(0xFF, 0xFF, 0xC0);

    /// Gets a system-defined color that has an ARGB value of #FFDDA0.
    public static Color Plum => new(0xFF, 0xDD, 0xA0);

    /// Gets a system-defined color that has an ARGB value of #FFB0E0.
    public static Color PowderBlue => new(0xFF, 0xB0, 0xE0);

    /// Gets a system-defined color that has an ARGB value of #FF8000.
    public static Color Purple => new(0xFF, 0x80, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFFF00.
    public static Color Red => new(0xFF, 0xFF, 0x00);

    /// Gets a system-defined color that has an ARGB value of #FFBC8F.
    public static Color RosyBrown => new(0xFF, 0xBC, 0x8F);

    /// Gets a system-defined color that has an ARGB value of #FF4169.
    public static Color RoyalBlue => new(0xFF, 0x41, 0x69);

    /// Gets a system-defined color that has an ARGB value of #FF8B45.
    public static Color SaddleBrown => new(0xFF, 0x8B, 0x45);

    /// Gets a system-defined color that has an ARGB value of #FFFA80.
    public static Color Salmon => new(0xFF, 0xFA, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FFF4A4.
    public static Color SandyBrown => new(0xFF, 0xF4, 0xA4);

    /// Gets a system-defined color that has an ARGB value of #FF2E8B.
    public static Color SeaGreen => new(0xFF, 0x2E, 0x8B);

    /// Gets a system-defined color that has an ARGB value of #FFFFF5.
    public static Color SeaShell => new(0xFF, 0xFF, 0xF5);

    /// Gets a system-defined color that has an ARGB value of #FFA052.
    public static Color Sienna => new(0xFF, 0xA0, 0x52);

    /// Gets a system-defined color that has an ARGB value of #FFC0C0.
    public static Color Silver => new(0xFF, 0xC0, 0xC0);

    /// Gets a system-defined color that has an ARGB value of #FF87CE.
    public static Color SkyBlue => new(0xFF, 0x87, 0xCE);

    /// Gets a system-defined color that has an ARGB value of #FF6A5A.
    public static Color SlateBlue => new(0xFF, 0x6A, 0x5A);

    /// Gets a system-defined color that has an ARGB value of #FF7080.
    public static Color SlateGray => new(0xFF, 0x70, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FFFFFA.
    public static Color Snow => new(0xFF, 0xFF, 0xFA);

    /// Gets a system-defined color that has an ARGB value of #FF00FF.
    public static Color SpringGreen => new(0xFF, 0x00, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF4682.
    public static Color SteelBlue => new(0xFF, 0x46, 0x82);

    /// Gets a system-defined color that has an ARGB value of #FFD2B4.
    public static Color Tan => new(0xFF, 0xD2, 0xB4);

    /// Gets a system-defined color that has an ARGB value of #FF0080.
    public static Color Teal => new(0xFF, 0x00, 0x80);

    /// Gets a system-defined color that has an ARGB value of #FFD8BF.
    public static Color Thistle => new(0xFF, 0xD8, 0xBF);

    /// Gets a system-defined color that has an ARGB value of #FFFF63.
    public static Color Tomato => new(0xFF, 0xFF, 0x63);

    /// Gets a system-defined color that has an ARGB value of #FF40E0.
    public static Color Turquoise => new(0xFF, 0x40, 0xE0);

    /// Gets a system-defined color that has an ARGB value of #FFEE82.
    public static Color Violet => new(0xFF, 0xEE, 0x82);

    /// Gets a system-defined color that has an ARGB value of #FFF5DE.
    public static Color Wheat => new(0xFF, 0xF5, 0xDE);

    /// Gets a system-defined color that has an ARGB value of #FFFFFF.
    public static Color White => new(0xFF, 0xFF, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FFF5F5.
    public static Color WhiteSmoke => new(0xFF, 0xF5, 0xF5);
    style = new ();

    /// Gets a system-defined color that has an ARGB value of #FFFFFF.
    public static Color Yellow => new(0xFF, 0xFF, 0xFF);

    /// Gets a system-defined color that has an ARGB value of #FF9ACD.
    public static Color YellowGreen => new(0xFF, 0x9A, 0xCD);
}

 A

Gets the alpha component value of this Color structure.
AliceBlue

Gets a system-defined color that has an ARGB value of #FFF0F8FF.
AntiqueWhite

Gets a system-defined color that has an ARGB value of #FFFAEBD7.
Aqua

Gets a system-defined color that has an ARGB value of #FF00FFFF.
Aquamarine

Gets a system-defined color that has an ARGB value of #FF7FFFD4.
Azure

Gets a system-defined color that has an ARGB value of #FFF0FFFF.
B

Gets the blue component value of this Color structure.
Beige

Gets a system-defined color that has an ARGB value of #FFF5F5DC.
Bisque

Gets a system-defined color that has an ARGB value of #FFFFE4C4.
Black

Gets a system-defined color that has an ARGB value of #FF000000.
BlanchedAlmond

Gets a system-defined color that has an ARGB value of #FFFFEBCD.
Blue

Gets a system-defined color that has an ARGB value of #FF0000FF.
BlueViolet

Gets a system-defined color that has an ARGB value of #FF8A2BE2.
Brown

Gets a system-defined color that has an ARGB value of #FFA52A2A.
BurlyWood

Gets a system-defined color that has an ARGB value of #FFDEB887.
CadetBlue

Gets a system-defined color that has an ARGB value of #FF5F9EA0.
Chartreuse

Gets a system-defined color that has an ARGB value of #FF7FFF00.
Chocolate

Gets a system-defined color that has an ARGB value of #FFD2691E.
Coral

Gets a system-defined color that has an ARGB value of #FFFF7F50.
CornflowerBlue

Gets a system-defined color that has an ARGB value of #FF6495ED.
Cornsilk

Gets a system-defined color that has an ARGB value of #FFFFF8DC.
Crimson

Gets a system-defined color that has an ARGB value of #FFDC143C.
Cyan

Gets a system-defined color that has an ARGB value of #FF00FFFF.
DarkBlue

Gets a system-defined color that has an ARGB value of #FF00008B.
DarkCyan

Gets a system-defined color that has an ARGB value of #FF008B8B.
DarkGoldenrod

Gets a system-defined color that has an ARGB value of #FFB8860B.
DarkGray

Gets a system-defined color that has an ARGB value of #FFA9A9A9.
DarkGreen

Gets a system-defined color that has an ARGB value of #FF006400.
DarkKhaki

Gets a system-defined color that has an ARGB value of #FFBDB76B.
DarkMagenta

Gets a system-defined color that has an ARGB value of #FF8B008B.
DarkOliveGreen

Gets a system-defined color that has an ARGB value of #FF556B2F.
DarkOrange

Gets a system-defined color that has an ARGB value of #FFFF8C00.
DarkOrchid

Gets a system-defined color that has an ARGB value of #FF9932CC.
DarkRed

Gets a system-defined color that has an ARGB value of #FF8B0000.
DarkSalmon

Gets a system-defined color that has an ARGB value of #FFE9967A.
DarkSeaGreen

Gets a system-defined color that has an ARGB value of #FF8FBC8F.
DarkSlateBlue

Gets a system-defined color that has an ARGB value of #FF483D8B.
DarkSlateGray

Gets a system-defined color that has an ARGB value of #FF2F4F4F.
DarkTurquoise

Gets a system-defined color that has an ARGB value of #FF00CED1.
DarkViolet

Gets a system-defined color that has an ARGB value of #FF9400D3.
DeepPink

Gets a system-defined color that has an ARGB value of #FFFF1493.
DeepSkyBlue

Gets a system-defined color that has an ARGB value of #FF00BFFF.
DimGray

Gets a system-defined color that has an ARGB value of #FF696969.
DodgerBlue

Gets a system-defined color that has an ARGB value of #FF1E90FF.
Firebrick

Gets a system-defined color that has an ARGB value of #FFB22222.
FloralWhite

Gets a system-defined color that has an ARGB value of #FFFFFAF0.
ForestGreen

Gets a system-defined color that has an ARGB value of #FF228B22.
Fuchsia

Gets a system-defined color that has an ARGB value of #FFFF00FF.
G

Gets the green component value of this Color structure.
Gainsboro

Gets a system-defined color that has an ARGB value of #FFDCDCDC.
GhostWhite

Gets a system-defined color that has an ARGB value of #FFF8F8FF.
Gold

Gets a system-defined color that has an ARGB value of #FFFFD700.
Goldenrod

Gets a system-defined color that has an ARGB value of #FFDAA520.
Gray

Gets a system-defined color that has an ARGB value of #FF808080.
Green

Gets a system-defined color that has an ARGB value of #FF008000.
GreenYellow

Gets a system-defined color that has an ARGB value of #FFADFF2F.
Honeydew

Gets a system-defined color that has an ARGB value of #FFF0FFF0.
HotPink

Gets a system-defined color that has an ARGB value of #FFFF69B4.
IndianRed

Gets a system-defined color that has an ARGB value of #FFCD5C5C.
Indigo

Gets a system-defined color that has an ARGB value of #FF4B0082.
IsEmpty

Specifies whether this Color structure is uninitialized.
IsKnownColor

Gets a value indicating whether this Color structure is a predefined color.Predefined colors are represented by the elements of the KnownColor enumeration.
IsNamedColor

Gets a value indicating whether this Color structure is a named color or a member of the KnownColor enumeration.
IsSystemColor

Gets a value indicating whether this Color structure is a system color.A system color is a color that is used in a Windows display element. System colors are represented by elements of the KnownColor enumeration.
Ivory

Gets a system-defined color that has an ARGB value of #FFFFFFF0.
Khaki

Gets a system-defined color that has an ARGB value of #FFF0E68C.
Lavender

Gets a system-defined color that has an ARGB value of #FFE6E6FA.
LavenderBlush

Gets a system-defined color that has an ARGB value of #FFFFF0F5.
LawnGreen

Gets a system-defined color that has an ARGB value of #FF7CFC00.
LemonChiffon

Gets a system-defined color that has an ARGB value of #FFFFFACD.
LightBlue

Gets a system-defined color that has an ARGB value of #FFADD8E6.
LightCoral

Gets a system-defined color that has an ARGB value of #FFF08080.
LightCyan

Gets a system-defined color that has an ARGB value of #FFE0FFFF.
LightGoldenrodYellow

Gets a system-defined color that has an ARGB value of #FFFAFAD2.
LightGray

Gets a system-defined color that has an ARGB value of #FFD3D3D3.
LightGreen

Gets a system-defined color that has an ARGB value of #FF90EE90.
LightPink

Gets a system-defined color that has an ARGB value of #FFFFB6C1.
LightSalmon

Gets a system-defined color that has an ARGB value of #FFFFA07A.
LightSeaGreen

Gets a system-defined color that has an ARGB value of #FF20B2AA.
LightSkyBlue

Gets a system-defined color that has an ARGB value of #FF87CEFA.
LightSlateGray

Gets a system-defined color that has an ARGB value of #FF778899.
LightSteelBlue

Gets a system-defined color that has an ARGB value of #FFB0C4DE.
LightYellow

Gets a system-defined color that has an ARGB value of #FFFFFFE0.
Lime

Gets a system-defined color that has an ARGB value of #FF00FF00.
LimeGreen

Gets a system-defined color that has an ARGB value of #FF32CD32.
Linen

Gets a system-defined color that has an ARGB value of #FFFAF0E6.
Magenta

Gets a system-defined color that has an ARGB value of #FFFF00FF.
Maroon

Gets a system-defined color that has an ARGB value of #FF800000.
MediumAquamarine

Gets a system-defined color that has an ARGB value of #FF66CDAA.
MediumBlue

Gets a system-defined color that has an ARGB value of #FF0000CD.
MediumOrchid

Gets a system-defined color that has an ARGB value of #FFBA55D3.
MediumPurple

Gets a system-defined color that has an ARGB value of #FF9370DB.
MediumSeaGreen

Gets a system-defined color that has an ARGB value of #FF3CB371.
MediumSlateBlue

Gets a system-defined color that has an ARGB value of #FF7B68EE.
MediumSpringGreen

Gets a system-defined color that has an ARGB value of #FF00FA9A.
MediumTurquoise

Gets a system-defined color that has an ARGB value of #FF48D1CC.
MediumVioletRed

Gets a system-defined color that has an ARGB value of #FFC71585.
MidnightBlue

Gets a system-defined color that has an ARGB value of #FF191970.
MintCream

Gets a system-defined color that has an ARGB value of #FFF5FFFA.
MistyRose

Gets a system-defined color that has an ARGB value of #FFFFE4E1.
Moccasin

Gets a system-defined color that has an ARGB value of #FFFFE4B5.
Name

Gets the name of this Color.
NavajoWhite

Gets a system-defined color that has an ARGB value of #FFFFDEAD.
Navy

Gets a system-defined color that has an ARGB value of #FF000080.
OldLace

Gets a system-defined color that has an ARGB value of #FFFDF5E6.
Olive

Gets a system-defined color that has an ARGB value of #FF808000.
OliveDrab

Gets a system-defined color that has an ARGB value of #FF6B8E23.
Orange

Gets a system-defined color that has an ARGB value of #FFFFA500.
OrangeRed

Gets a system-defined color that has an ARGB value of #FFFF4500.
Orchid

Gets a system-defined color that has an ARGB value of #FFDA70D6.
PaleGoldenrod

Gets a system-defined color that has an ARGB value of #FFEEE8AA.
PaleGreen

Gets a system-defined color that has an ARGB value of #FF98FB98.
PaleTurquoise

Gets a system-defined color that has an ARGB value of #FFAFEEEE.
PaleVioletRed

Gets a system-defined color that has an ARGB value of #FFDB7093.
PapayaWhip

Gets a system-defined color that has an ARGB value of #FFFFEFD5.
PeachPuff

Gets a system-defined color that has an ARGB value of #FFFFDAB9.
Peru

Gets a system-defined color that has an ARGB value of #FFCD853F.
Pink

Gets a system-defined color that has an ARGB value of #FFFFC0CB.
Plum

Gets a system-defined color that has an ARGB value of #FFDDA0DD.
PowderBlue

Gets a system-defined color that has an ARGB value of #FFB0E0E6.
Purple

Gets a system-defined color that has an ARGB value of #FF800080.
R

Gets the red component value of this Color structure.
RebeccaPurple

Gets a system-defined color that has an ARGB value of #663399.
Red

Gets a system-defined color that has an ARGB value of #FFFF0000.
RosyBrown

Gets a system-defined color that has an ARGB value of #FFBC8F8F.
RoyalBlue

Gets a system-defined color that has an ARGB value of #FF4169E1.
SaddleBrown

Gets a system-defined color that has an ARGB value of #FF8B4513.
Salmon

Gets a system-defined color that has an ARGB value of #FFFA8072.
SandyBrown

Gets a system-defined color that has an ARGB value of #FFF4A460.
SeaGreen

Gets a system-defined color that has an ARGB value of #FF2E8B57.
SeaShell

Gets a system-defined color that has an ARGB value of #FFFFF5EE.
Sienna

Gets a system-defined color that has an ARGB value of #FFA0522D.
Silver

Gets a system-defined color that has an ARGB value of #FFC0C0C0.
SkyBlue

Gets a system-defined color that has an ARGB value of #FF87CEEB.
SlateBlue

Gets a system-defined color that has an ARGB value of #FF6A5ACD.
SlateGray

Gets a system-defined color that has an ARGB value of #FF708090.
Snow

Gets a system-defined color that has an ARGB value of #FFFFFAFA.
SpringGreen

Gets a system-defined color that has an ARGB value of #FF00FF7F.
SteelBlue

Gets a system-defined color that has an ARGB value of #FF4682B4.
Tan

Gets a system-defined color that has an ARGB value of #FFD2B48C.
Teal

Gets a system-defined color that has an ARGB value of #FF008080.
Thistle

Gets a system-defined color that has an ARGB value of #FFD8BFD8.
Tomato

Gets a system-defined color that has an ARGB value of #FFFF6347.
Transparent

Gets a system-defined color.
Turquoise

Gets a system-defined color that has an ARGB value of #FF40E0D0.
Violet

Gets a system-defined color that has an ARGB value of #FFEE82EE.
Wheat

Gets a system-defined color that has an ARGB value of #FFF5DEB3.
White

Gets a system-defined color that has an ARGB value of #FFFFFFFF.
WhiteSmoke

Gets a system-defined color that has an ARGB value of #FFF5F5F5.
Yellow

Gets a system-defined color that has an ARGB value of #FFFFFF00.
YellowGreen

Gets a system-defined color that has an ARGB value of #FF9ACD32.
}

