using NUnit.Framework;
using Scoria.Rendering;
using Scoria.Styling;

namespace Scoria.Tests;

[TestFixture]
public class SurfaceTests
{
    [Test]
    public void Constructor_SetsWidthAndHeight()
    {
        Surface surface = new Surface(10, 5, Theme.Default);
        Assert.That(surface.Width, Is.EqualTo(10));
        Assert.That(surface.Height, Is.EqualTo(5));
    }

    [Test]
    public void Write_StoresCharacter()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        surface.Write('A', 2, 3, new Style());
        Assert.That(surface.GetChar(2, 3), Is.EqualTo('A'));
    }

    [Test]
    public void Write_StoresStyle()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style style = new Style(new Color(100, 200, 50), StyleAttributes.None);
        surface.Write('X', 1, 1, style);
        Style result = surface.GetStyle(1, 1);
        Assert.That(result.Foreground.R, Is.EqualTo(100));
        Assert.That(result.Foreground.G, Is.EqualTo(200));
        Assert.That(result.Foreground.B, Is.EqualTo(50));
    }

    [Test]
    public void Write_OverwritesPreviousCharacter()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        surface.Write('A', 0, 0, new Style());
        surface.Write('B', 0, 0, new Style());
        Assert.That(surface.GetChar(0, 0), Is.EqualTo('B'));
    }

    [Test]
    public void Write_OpaqueAlpha_DoesNotBlend()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style oldStyle = new Style(new Color(100, 100, 100), new Color(50, 50, 50), Style.MaxAlpha, StyleAttributes.None);
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(new Color(200, 200, 200), new Color(200, 200, 200), Style.MaxAlpha, StyleAttributes.None);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.Background.R, Is.EqualTo(200));
        Assert.That(result.Background.G, Is.EqualTo(200));
        Assert.That(result.Background.B, Is.EqualTo(200));
    }

    [Test]
    public void Write_PartialAlpha_BlendsBackground()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style oldStyle = new Style(new Color(0, 0, 0), new Color(100, 100, 100));
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(new Color(0, 0, 0), new Color(200, 200, 200), 128);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        // Alpha=128 -> blended background ≈ (200*128 + 100*127 + 127) / 255 ≈ 150
        Assert.That(result.Background.R, Is.EqualTo(150));
        Assert.That(result.Background.G, Is.EqualTo(150));
        Assert.That(result.Background.B, Is.EqualTo(150));
    }

    [Test]
    public void Write_FullTransparentAlpha_BlendsCompletelyWithOld()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style oldStyle = new Style(new Color(0, 0, 0), new Color(80, 80, 80));
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(new Color(0, 0, 0), new Color(200, 200, 200), 0);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.Background.R, Is.EqualTo(80));
        Assert.That(result.Background.G, Is.EqualTo(80));
        Assert.That(result.Background.B, Is.EqualTo(80));
    }

    [Test]
    public void Write_ZeroAlpha_KeepsOldBackground()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style oldStyle = new Style(new Color(0, 0, 0), new Color(42, 42, 42));
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(new Color(0, 0, 0), new Color(99, 99, 99), 0);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.Background.R, Is.EqualTo(42));
    }

    [Test]
    public void GetChar_UninitializedCell_ReturnsNullChar()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Assert.That(surface.GetChar(0, 0), Is.EqualTo('\0'));
    }

    [Test]
    public void GetStyle_UninitializedCell_ReturnsDefaultStyle()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style result = surface.GetStyle(0, 0);
        Assert.That(result, Is.EqualTo(default(Style)));
    }

    [Test]
    public void Write_DifferentCells_Independent()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        Style style1 = new Style(new Color(10, 0, 0), StyleAttributes.None);
        Style style2 = new Style(new Color(0, 20, 0), StyleAttributes.None);
        surface.Write('A', 0, 0, style1);
        surface.Write('B', 1, 1, style2);

        Assert.That(surface.GetChar(0, 0), Is.EqualTo('A'));
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('B'));
        Assert.That(surface.GetStyle(0, 0).Foreground.R, Is.EqualTo(10));
        Assert.That(surface.GetStyle(1, 1).Foreground.G, Is.EqualTo(20));
    }

    [Test]
    public void ImplementsISurface()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        Assert.That(surface, Is.InstanceOf<ISurface>());
    }

    [Test]
    public void Write_OutOfBounds_DoesNothing()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        surface.Write('A', -1, 0, new Style());
        surface.Write('A', 0, -1, new Style());
        surface.Write('A', 3, 0, new Style());
        surface.Write('A', 0, 3, new Style());

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Assert.That(surface.GetChar(x, y), Is.EqualTo('\0'));
            }
        }
    }
}
