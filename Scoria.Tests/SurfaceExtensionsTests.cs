using NUnit.Framework;
using Scoria.Layout;
using Scoria.Rendering;
using Scoria.Styling;

namespace Scoria.Tests;

[TestFixture]
public class SurfaceExtensionsTests
{
    [Test]
    public void Fill_EntireSurface_FillsAllCells()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        Style style = new Style(new Color(10, 20, 30), StyleAttributes.None);
        surface.Fill('X', style);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Assert.That(surface.GetChar(x, y), Is.EqualTo('X'));
                Assert.That(surface.GetStyle(x, y).Foreground.R, Is.EqualTo(10));
            }
        }
    }

    [Test]
    public void Fill_EntireSurface_DoesNotAffectBeyondBounds()
    {
        Surface surface = new Surface(5, 5, Theme.Default);
        surface.Fill('A', new Style());
        surface.SubSurface(1, 1, 3, 3).Fill('B', new Style());

        Assert.That(surface.GetChar(0, 0), Is.EqualTo('A'));
        Assert.That(surface.GetChar(4, 4), Is.EqualTo('A'));
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('B'));
        Assert.That(surface.GetChar(2, 2), Is.EqualTo('B'));
    }

    [Test]
    public void Write_Composite_CopiesSourceToTarget()
    {
        Surface target = new Surface(5, 5, Theme.Default);
        Surface source = new Surface(3, 3, Theme.Default);
        source.Fill('S', new Style(new Color(10, 20, 30), StyleAttributes.None));

        target.Write(source, 1, 1);

        Assert.That(target.GetChar(1, 1), Is.EqualTo('S'));
        Assert.That(target.GetChar(2, 2), Is.EqualTo('S'));
        Assert.That(target.GetChar(3, 3), Is.EqualTo('S'));
        Assert.That(target.GetStyle(1, 1).Foreground.R, Is.EqualTo(10));
    }

    [Test]
    public void Write_Composite_DoesNotWriteOutsideTargetBounds()
    {
        Surface target = new Surface(3, 3, Theme.Default);
        Surface source = new Surface(5, 5, Theme.Default);
        source.Fill('S', new Style());

        target.Write(source, 0, 0);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Assert.That(target.GetChar(x, y), Is.EqualTo('S'));
            }
        }
    }

    [Test]
    public void Write_Composite_NegativeOffset_ClampsCorrectly()
    {
        Surface target = new Surface(5, 5, Theme.Default);
        Surface source = new Surface(3, 3, Theme.Default);
        source.Fill('S', new Style());

        target.Write(source, -1, -1);

        Assert.That(target.GetChar(0, 0), Is.EqualTo('S'));
        Assert.That(target.GetChar(1, 1), Is.EqualTo('S'));
        Assert.That(target.GetChar(2, 2), Is.EqualTo('\0'));
    }

    [Test]
    public void Write_Composite_PartialOverlap()
    {
        Surface target = new Surface(4, 4, Theme.Default);
        Surface source = new Surface(4, 4, Theme.Default);
        source.Fill('S', new Style());

        target.Write(source, 2, 2);

        Assert.That(target.GetChar(0, 0), Is.EqualTo('\0'));
        Assert.That(target.GetChar(2, 2), Is.EqualTo('S'));
        Assert.That(target.GetChar(3, 3), Is.EqualTo('S'));
    }

    [Test]
    public void Contains_InsideBounds_ReturnsTrue()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        Assert.That(surface.Contains(0, 0), Is.True);
        Assert.That(surface.Contains(2, 2), Is.True);
    }

    [Test]
    public void Contains_OutsideBounds_ReturnsFalse()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        Assert.That(surface.Contains(-1, 0), Is.False);
        Assert.That(surface.Contains(0, -1), Is.False);
        Assert.That(surface.Contains(3, 0), Is.False);
        Assert.That(surface.Contains(0, 3), Is.False);
    }

    [Test]
    public void Fill_WithForegroundAndBackground_SetsColors()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Fill('F', new Color(10, 20, 30), new Color(40, 50, 60));

        Style style = surface.GetStyle(1, 1);
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('F'));
        Assert.That(style.Foreground, Is.EqualTo(new Color(10, 20, 30)));
        Assert.That(style.Background, Is.EqualTo(new Color(40, 50, 60)));
    }

    [Test]
    public void Fill_WithStyleAttributes_AppliesAttributes()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Fill('F', new Color(10, 20, 30), new Color(40, 50, 60), styleAttributes: StyleAttributes.Bold);

        Assert.That(surface.GetStyle(1, 1).StyleAttributes, Is.EqualTo(StyleAttributes.Bold));
    }

    [Test]
    public void Fill_ForegroundOnly_PreservesExistingBackground()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Fill(new Color(0, 0, 100));
        surface.Fill('F', new Color(200, 200, 200));

        Style style = surface.GetStyle(0, 0);
        Assert.That(surface.GetChar(0, 0), Is.EqualTo('F'));
        Assert.That(style.Foreground, Is.EqualTo(new Color(200, 200, 200)));
        Assert.That(style.Background, Is.EqualTo(new Color(0, 0, 100)));
    }

    [Test]
    public void Fill_BackgroundOnly_FillsBackground()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Fill(new Color(10, 20, 30));

        Assert.That(surface.GetChar(1, 1), Is.EqualTo(Surface.BackgroundCharacter));
        Assert.That(surface.GetStyle(1, 1).Background, Is.EqualTo(new Color(10, 20, 30)));
    }

    [Test]
    public void Write_WithForegroundAndBackground_SetsColors()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Write('W', 1, 1, new Color(10, 20, 30), new Color(40, 50, 60));

        Style style = surface.GetStyle(1, 1);
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('W'));
        Assert.That(style.Foreground, Is.EqualTo(new Color(10, 20, 30)));
        Assert.That(style.Background, Is.EqualTo(new Color(40, 50, 60)));
    }

    [Test]
    public void Write_ForegroundOnly_PreservesExistingBackground()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Write(1, 1, new Color(0, 0, 100));
        surface.Write('W', 1, 1, new Color(200, 200, 200));

        Style style = surface.GetStyle(1, 1);
        Assert.That(surface.GetChar(1, 1), Is.EqualTo('W'));
        Assert.That(style.Foreground, Is.EqualTo(new Color(200, 200, 200)));
        Assert.That(style.Background, Is.EqualTo(new Color(0, 0, 100)));
    }

    [Test]
    public void Write_BackgroundOnly_SetsBackgroundColor()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Write(1, 1, new Color(10, 20, 30));

        Assert.That(surface.GetStyle(1, 1).Background, Is.EqualTo(new Color(10, 20, 30)));
    }

    [Test]
    public void Write_BackgroundOnly_PartialAlpha_KeepsCharacter()
    {
        Surface surface = new Surface(2, 2, Theme.Default);
        surface.Write('W', 1, 1, new Style());
        surface.Write(1, 1, new Color(10, 20, 30), 128);

        Assert.That(surface.GetChar(1, 1), Is.EqualTo('W'));
    }

    [Test]
    public void SubSurface_CalculatedLayout_CreatesCorrectSubSurface()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        CalculatedLayout layout = new CalculatedLayout();
        layout.SetProperty(LayoutPropertyType.X, 2);
        layout.SetProperty(LayoutPropertyType.Y, 3);
        layout.SetProperty(LayoutPropertyType.Width, 5);
        layout.SetProperty(LayoutPropertyType.Height, 4);

        SubSurface sub = parent.SubSurface(layout);

        Assert.That(sub.OffsetX, Is.EqualTo(2));
        Assert.That(sub.OffsetY, Is.EqualTo(3));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(4));
    }
}
