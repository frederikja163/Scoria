using NUnit.Framework;

namespace Scoria.Tests;

[TestFixture]
public class SurfaceTests
{
    [Test]
    public void Constructor_SetsWidthAndHeight()
    {
        Surface surface = new Surface(10, 5);
        Assert.That(surface.Width, Is.EqualTo(10));
        Assert.That(surface.Height, Is.EqualTo(5));
    }

    [Test]
    public void Write_StoresCharacter()
    {
        Surface surface = new Surface(5, 5);
        surface.Write('A', 2, 3, new Style());
        Assert.That(surface.GetChar(2, 3), Is.EqualTo('A'));
    }

    [Test]
    public void Write_StoresStyle()
    {
        Surface surface = new Surface(5, 5);
        Style style = new Style(100, 200, 50, StyleAttributes.None);
        surface.Write('X', 1, 1, style);
        Style result = surface.GetStyle(1, 1);
        Assert.That(result.ForegroundRed, Is.EqualTo(100));
        Assert.That(result.ForegroundGreen, Is.EqualTo(200));
        Assert.That(result.ForegroundBlue, Is.EqualTo(50));
    }

    [Test]
    public void Write_OverwritesPreviousCharacter()
    {
        Surface surface = new Surface(5, 5);
        surface.Write('A', 0, 0, new Style());
        surface.Write('B', 0, 0, new Style());
        Assert.That(surface.GetChar(0, 0), Is.EqualTo('B'));
    }

    [Test]
    public void Write_OpaqueAlpha_DoesNotBlend()
    {
        Surface surface = new Surface(5, 5);
        Style oldStyle = new Style(100, 100, 100, 50, 50, 50, 255, StyleAttributes.None);
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(200, 200, 200, 200, 200, 200, 255, StyleAttributes.None);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.BackgroundRed, Is.EqualTo(200));
        Assert.That(result.BackgroundGreen, Is.EqualTo(200));
        Assert.That(result.BackgroundBlue, Is.EqualTo(200));
    }

    [Test]
    public void Write_PartialAlpha_BlendsBackground()
    {
        Surface surface = new Surface(5, 5);
        Style oldStyle = new Style(0, 0, 0, 100, 100, 100);
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(0, 0, 0, 200, 200, 200, 128);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        // Alpha=128 -> blendNew ≈ 0.502, blendOld ≈ 0.498
        // BackgroundRed ≈ 200*0.502 + 100*0.498 ≈ 150
        Assert.That(result.BackgroundRed, Is.EqualTo(150));
        Assert.That(result.BackgroundGreen, Is.EqualTo(150));
        Assert.That(result.BackgroundBlue, Is.EqualTo(150));
    }

    [Test]
    public void Write_FullTransparentAlpha_BlendsCompletelyWithOld()
    {
        Surface surface = new Surface(5, 5);
        Style oldStyle = new Style(0, 0, 0, 80, 80, 80);
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(0, 0, 0, 200, 200, 200, 0);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.BackgroundRed, Is.EqualTo(80));
        Assert.That(result.BackgroundGreen, Is.EqualTo(80));
        Assert.That(result.BackgroundBlue, Is.EqualTo(80));
    }

    [Test]
    public void Write_ZeroAlpha_KeepsOldBackground()
    {
        Surface surface = new Surface(5, 5);
        Style oldStyle = new Style(0, 0, 0, 42, 42, 42);
        surface.Write('A', 0, 0, oldStyle);

        Style newStyle = new Style(0, 0, 0, 99, 99, 99, 0);
        surface.Write('B', 0, 0, newStyle);

        Style result = surface.GetStyle(0, 0);
        Assert.That(result.BackgroundRed, Is.EqualTo(42));
    }

    [Test]
    public void GetChar_UninitializedCell_ReturnsNullChar()
    {
        Surface surface = new Surface(5, 5);
        Assert.That(surface.GetChar(0, 0), Is.EqualTo('\0'));
    }

    [Test]
    public void GetStyle_UninitializedCell_ReturnsDefaultStyle()
    {
        Surface surface = new Surface(5, 5);
        Style result = surface.GetStyle(0, 0);
        Assert.That(result, Is.EqualTo(default(Style)));
    }

    [Test]
    public void Write_DifferentCells_Independent()
    {
        Surface surface = new Surface(5, 5);
        Style style1 = new Style(10, 0, 0, StyleAttributes.None);
        Style style2 = new Style(0, 20, 0, StyleAttributes.None);
        surface.Write('A', 0, 0, style1);
        surface.Write('B', 1, 1, style2);

        Assert.That(surface.GetChar(0, 0), Is.EqualTo('A'));
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('B'));
        Assert.That(surface.GetStyle(0, 0).ForegroundRed, Is.EqualTo(10));
        Assert.That(surface.GetStyle(1, 1).ForegroundGreen, Is.EqualTo(20));
    }

    [Test]
    public void ImplementsISurface()
    {
        Surface surface = new Surface(3, 3);
        Assert.That(surface, Is.InstanceOf<ISurface>());
    }
}
