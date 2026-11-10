using NUnit.Framework;
using Scoria.Styling;

namespace Scoria.Tests;

[TestFixture]
public class ColorTests
{
    [Test]
    public void Constructor_SetsChannels()
    {
        Color color = new Color(10, 20, 30);
        Assert.That(color.R, Is.EqualTo(10));
        Assert.That(color.G, Is.EqualTo(20));
        Assert.That(color.B, Is.EqualTo(30));
    }

    [Test]
    public void Constructor_Grayscale_SetsAllChannels()
    {
        Color color = new Color(128);
        Assert.That(color.R, Is.EqualTo(128));
        Assert.That(color.G, Is.EqualTo(128));
        Assert.That(color.B, Is.EqualTo(128));
    }

    [Test]
    public void Blend_OpaqueAlpha_ReturnsThisColor()
    {
        Color color = new Color(10, 20, 30);
        Color result = color.Blend(new Color(200, 200, 200), Style.MaxAlpha);
        Assert.That(result, Is.EqualTo(color));
    }

    [Test]
    public void Blend_TransparentAlpha_ReturnsUnderneathColor()
    {
        Color underneath = new Color(40, 50, 60);
        Color result = new Color(10, 20, 30).Blend(underneath, Style.MinAlpha);
        Assert.That(result, Is.EqualTo(underneath));
    }

    [Test]
    public void Blend_HalfAlpha_BlendsTowardUnderneath()
    {
        Color result = new Color(200, 200, 200).Blend(new Color(100, 100, 100), Style.HalfAlpha);
        Assert.That(result.R, Is.EqualTo(150));
        Assert.That(result.G, Is.EqualTo(150));
        Assert.That(result.B, Is.EqualTo(150));
    }

    [Test]
    public void Operator_Add_AddsChannels()
    {
        Color result = new Color(10, 20, 30) + new Color(1, 2, 3);
        Assert.That(result, Is.EqualTo(new Color(11, 22, 33)));
    }

    [Test]
    public void Operator_Subtract_SubtractsChannels()
    {
        Color result = new Color(10, 20, 30) - new Color(1, 2, 3);
        Assert.That(result, Is.EqualTo(new Color(9, 18, 27)));
    }

    [Test]
    public void Operator_Multiply_ScalesChannels()
    {
        Color result = new Color(10, 20, 30) * 2f;
        Assert.That(result, Is.EqualTo(new Color(20, 40, 60)));
    }

    [Test]
    public void Operator_Multiply_FactorFirst_ScalesChannels()
    {
        Color result = 2f * new Color(10, 20, 30);
        Assert.That(result, Is.EqualTo(new Color(20, 40, 60)));
    }

    [Test]
    public void Equality_SameChannels_AreEqual()
    {
        Assert.That(new Color(1, 2, 3) == new Color(1, 2, 3), Is.True);
    }

    [Test]
    public void Equality_DifferentChannels_AreNotEqual()
    {
        Assert.That(new Color(1, 2, 3) == new Color(3, 2, 1), Is.False);
    }

    [Test]
    public void Equals_SameChannels_ReturnsTrue()
    {
        Assert.That(new Color(1, 2, 3).Equals(new Color(1, 2, 3)), Is.True);
    }

    [Test]
    public void GetHashCode_SameChannels_SameHashCode()
    {
        Assert.That(new Color(1, 2, 3).GetHashCode(), Is.EqualTo(new Color(1, 2, 3).GetHashCode()));
    }
}
