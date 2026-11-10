using NUnit.Framework;
using Scoria.Styling;

namespace Scoria.Tests;

[TestFixture]
public class StyleTests
{
    [Test]
    public void MaxAlpha_Is255()
    {
        Assert.That(Style.MaxAlpha, Is.EqualTo(byte.MaxValue));
    }

    [Test]
    public void MinAlpha_Is0()
    {
        Assert.That(Style.MinAlpha, Is.EqualTo(0));
    }

    [Test]
    public void HalfAlpha_Is127()
    {
        Assert.That(Style.HalfAlpha, Is.EqualTo(127));
    }

    [Test]
    public void ForegroundAndBackground_AreSettable()
    {
        Style style = new Style
        {
            Foreground = new Color(10, 20, 30),
            Background = new Color(40, 50, 60),
        };

        Assert.That(style.Foreground, Is.EqualTo(new Color(10, 20, 30)));
        Assert.That(style.Background, Is.EqualTo(new Color(40, 50, 60)));
    }

    [Test]
    public void Constructor_ForegroundOnly_UsesBlackBackground()
    {
        Style style = new Style(new Color(1, 2, 3));
        Assert.That(style.Foreground, Is.EqualTo(new Color(1, 2, 3)));
        Assert.That(style.Background, Is.EqualTo(Color.Black));
    }

    [Test]
    public void Constructor_Full_SetsAllChannels()
    {
        Style style = new Style(new Color(1, 2, 3), new Color(4, 5, 6), 128, StyleAttributes.Bold);
        Assert.That(style.Foreground, Is.EqualTo(new Color(1, 2, 3)));
        Assert.That(style.Background, Is.EqualTo(new Color(4, 5, 6)));
        Assert.That(style.Alpha, Is.EqualTo(128));
        Assert.That(style.StyleAttributes, Is.EqualTo(StyleAttributes.Bold));
    }

    [Test]
    public void DefaultStyle_HasBlackColorsAndMaxAlpha()
    {
        Style style = new Style();
        Assert.That(style.Foreground, Is.EqualTo(Color.Black));
        Assert.That(style.Background, Is.EqualTo(Color.Black));
        Assert.That(style.Alpha, Is.EqualTo(Style.MaxAlpha));
        Assert.That(style.StyleAttributes, Is.EqualTo(StyleAttributes.None));
    }
}
