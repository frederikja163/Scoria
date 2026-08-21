using NUnit.Framework;

namespace Scoria.Tests;

[TestFixture]
public class SubSurfaceTests
{
    [Test]
    public void Properties_ReflectConstructorArguments()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 2, 3, 5, 4, parent.Theme);

        Assert.That(sub.OffsetX, Is.EqualTo(2));
        Assert.That(sub.OffsetY, Is.EqualTo(3));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(4));
    }

    [Test]
    public void Write_ForwardsToParentWithOffset()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 2, 3, 5, 5, parent.Theme);

        sub.Write('A', 0, 0, new Style());

        Assert.That(parent.GetChar(2, 3), Is.EqualTo('A'));
    }

    [Test]
    public void GetChar_ReadsFromParentWithOffset()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        parent.Write('X', 4, 5, new Style());
        SubSurface sub = new SubSurface(parent, 2, 3, 5, 5, parent.Theme);

        Assert.That(sub.GetChar(2, 2), Is.EqualTo('X'));
    }

    [Test]
    public void GetStyle_ReadsFromParentWithOffset()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Style style = new Style(10, 20, 30, StyleAttributes.None);
        parent.Write('X', 3, 4, style);
        SubSurface sub = new SubSurface(parent, 1, 2, 5, 5, parent.Theme);

        Style result = sub.GetStyle(2, 2);
        Assert.That(result.ForegroundRed, Is.EqualTo(10));
        Assert.That(result.ForegroundGreen, Is.EqualTo(20));
        Assert.That(result.ForegroundBlue, Is.EqualTo(30));
    }

    [Test]
    public void Write_MultipleCharacters_AllForwardedToParent()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 1, 1, 3, 3, parent.Theme);

        sub.Write('A', 0, 0, new Style());
        sub.Write('B', 1, 1, new Style());
        sub.Write('C', 2, 2, new Style());

        Assert.That(parent.GetChar(1, 1), Is.EqualTo('A'));
        Assert.That(parent.GetChar(2, 2), Is.EqualTo('B'));
        Assert.That(parent.GetChar(3, 3), Is.EqualTo('C'));
    }

    [Test]
    public void ImplementsISurface()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.That(sub, Is.InstanceOf<ISurface>());
    }

    [Test]
    public void Write_NegativeX_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.Write('A', -1, 0, new Style()));
    }

    [Test]
    public void Write_NegativeY_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.Write('A', 0, -1, new Style()));
    }

    [Test]
    public void Write_XEqualToWidth_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.Write('A', 5, 0, new Style()));
    }

    [Test]
    public void Write_YEqualToHeight_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.Write('A', 0, 5, new Style()));
    }

    [Test]
    public void GetChar_NegativeX_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.GetChar(-1, 0));
    }

    [Test]
    public void GetChar_YEqualToHeight_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.GetChar(0, 5));
    }

    [Test]
    public void GetStyle_NegativeX_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.GetStyle(-1, 0));
    }

    [Test]
    public void GetStyle_XEqualToWidth_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 0, 0, 5, 5, parent.Theme);
        Assert.Throws<ArgumentOutOfRangeException>(() => sub.GetStyle(5, 0));
    }

    [Test]
    public void Constructor_NegativeOffsetX_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, -1, 0, 5, 5, parent.Theme));
    }

    [Test]
    public void Constructor_NegativeOffsetY_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, 0, -1, 5, 5, parent.Theme));
    }

    [Test]
    public void Constructor_CanTouchParentEdge()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 5, 0, 5, 5, parent.Theme);

        Assert.That(sub.OffsetX, Is.EqualTo(5));
        Assert.That(sub.OffsetY, Is.EqualTo(0));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(5));
    }

    [Test]
    public void Constructor_ZeroWidth_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, 0, 0, 0, 5, parent.Theme));
    }

    [Test]
    public void Constructor_ZeroHeight_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, 0, 0, 5, 0, parent.Theme));
    }

    [Test]
    public void Constructor_SubSurfaceExceedsParentWidth_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, 8, 0, 5, 5, parent.Theme));
    }

    [Test]
    public void Constructor_SubSurfaceExceedsParentHeight_ThrowsArgumentOutOfRangeException()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        Assert.Throws<ArgumentOutOfRangeException>(() => new SubSurface(parent, 0, 8, 5, 5, parent.Theme));
    }

    [Test]
    public void Fill_Extension_FillsSubSurfaceRegion()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 2, 2, 3, 3, parent.Theme);
        sub.Fill('F', new Style());

        Assert.That(sub.GetChar(0, 0), Is.EqualTo('F'));
        Assert.That(sub.GetChar(2, 2), Is.EqualTo('F'));
        Assert.That(parent.GetChar(2, 2), Is.EqualTo('F'));
        Assert.That(parent.GetChar(4, 4), Is.EqualTo('F'));
    }

    [Test]
    public void Fill_Extension_FillsNestedSubSurfaceRegion()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = new SubSurface(parent, 2, 2, 5, 5, parent.Theme);
        sub.SubSurface(1, 1, 3, 3).Fill('R', new Style());

        Assert.That(sub.GetChar(0, 0), Is.EqualTo('\0'));
        Assert.That(sub.GetChar(1, 1), Is.EqualTo('R'));
        Assert.That(sub.GetChar(2, 2), Is.EqualTo('R'));
    }

    [Test]
    public void SubSurface_Extension_CreatesCorrectSubSurface()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(2, 3, 5, 4);

        Assert.That(sub.OffsetX, Is.EqualTo(2));
        Assert.That(sub.OffsetY, Is.EqualTo(3));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(4));
    }

    [Test]
    public void SubSurface_Extension_ForwardsWritesToParent()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(1, 2, 5, 5);
        sub.Write('A', 0, 0, new Style());

        Assert.That(parent.GetChar(1, 2), Is.EqualTo('A'));
    }

    [Test]
    public void SubSurface_Extension_ReadsFromParent()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        parent.Write('X', 3, 4, new Style());
        SubSurface sub = parent.SubSurface(1, 2, 5, 5);

        Assert.That(sub.GetChar(2, 2), Is.EqualTo('X'));
    }

    [Test]
    public void SubSurface_Range_CreatesCorrectSubSurface()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(2..7, 3..8);

        Assert.That(sub.OffsetX, Is.EqualTo(2));
        Assert.That(sub.OffsetY, Is.EqualTo(3));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(5));
    }

    [Test]
    public void SubSurface_Range_FromStart()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(..5, ..3);

        Assert.That(sub.OffsetX, Is.EqualTo(0));
        Assert.That(sub.OffsetY, Is.EqualTo(0));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(3));
    }

    [Test]
    public void SubSurface_Range_ToEnd()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(2..7, 3..8);

        Assert.That(sub.OffsetX, Is.EqualTo(2));
        Assert.That(sub.OffsetY, Is.EqualTo(3));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(5));
    }

    [Test]
    public void SubSurface_Range_FullRange()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(0..5, 0..5);

        Assert.That(sub.OffsetX, Is.EqualTo(0));
        Assert.That(sub.OffsetY, Is.EqualTo(0));
        Assert.That(sub.Width, Is.EqualTo(5));
        Assert.That(sub.Height, Is.EqualTo(5));
    }

    [Test]
    public void SubSurface_Range_ForwardsWritesToParent()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(2..7, 3..8);
        sub.Write('B', 0, 0, new Style());

        Assert.That(parent.GetChar(2, 3), Is.EqualTo('B'));
    }

    [Test]
    public void SubSurface_Range_ReadsFromParent()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        parent.Write('Y', 4, 5, new Style());
        SubSurface sub = parent.SubSurface(2..7, 3..8);

        Assert.That(sub.GetChar(2, 2), Is.EqualTo('Y'));
    }

    [Test]
    public void SubSurface_Range_IsInstanceofISurface()
    {
        Surface parent = new Surface(10, 10, Theme.Default);
        SubSurface sub = parent.SubSurface(0..5, 0..5);
        Assert.That(sub, Is.InstanceOf<ISurface>());
    }
}
