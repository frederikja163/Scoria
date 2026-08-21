using NUnit.Framework;

namespace Scoria.Tests;

[TestFixture]
public class SurfaceExtensionsTests
{
    [Test]
    public void Fill_EntireSurface_FillsAllCells()
    {
        Surface surface = new Surface(3, 3, Theme.Default);
        Style style = new Style(10, 20, 30, StyleAttributes.None);
        surface.Fill('X', style);

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Assert.That(surface.GetChar(x, y), Is.EqualTo('X'));
                Assert.That(surface.GetStyle(x, y).ForegroundRed, Is.EqualTo(10));
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
        source.Fill('S', new Style(10, 20, 30, StyleAttributes.None));

        target.Write(source, 1, 1);

        Assert.That(target.GetChar(1, 1), Is.EqualTo('S'));
        Assert.That(target.GetChar(2, 2), Is.EqualTo('S'));
        Assert.That(target.GetChar(3, 3), Is.EqualTo('S'));
        Assert.That(target.GetStyle(1, 1).ForegroundRed, Is.EqualTo(10));
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
}
