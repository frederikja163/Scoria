using NUnit.Framework;
using Scoria.Events;

namespace Scoria.Tests.Events;

[TestFixture]
public class KeyTests
{
    [Test]
    public void None_Base_ReturnsNone()
    {
        Assert.That(Key.None.Base(), Is.EqualTo(Key.None));
    }

    [Test]
    public void Base_StripsCtrl()
    {
        Assert.That((Key.A | Key.Ctrl).Base(), Is.EqualTo(Key.A));
    }

    [Test]
    public void Base_StripsAlt()
    {
        Assert.That((Key.B | Key.Alt).Base(), Is.EqualTo(Key.B));
    }

    [Test]
    public void Base_StripsShift()
    {
        Assert.That((Key.C | Key.Shift).Base(), Is.EqualTo(Key.C));
    }

    [Test]
    public void Base_StripsAllModifiers()
    {
        Assert.That((Key.D | Key.Ctrl | Key.Alt | Key.Shift).Base(), Is.EqualTo(Key.D));
    }

    [Test]
    public void Base_UnmodifiedKey_ReturnsSame()
    {
        Assert.That(Key.F5.Base(), Is.EqualTo(Key.F5));
    }
}
