using NUnit.Framework;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class FocusInputProviderTests
{
    private FocusInputProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new FocusInputProvider();
    }

    [Test]
    public void Enable_ReturnsTrue()
    {
        Assert.That(_provider.Enable, Is.True);
    }

    [Test]
    public void Order_ReturnsZero()
    {
        Assert.That(_provider.Order, Is.EqualTo(0));
    }

    [Test]
    public void HandleInput_FocusGained_ReturnsFocusedTrue()
    {
        EventArgs? result = _provider.HandleInput("\x1b[I");
        Assert.That(result, Is.TypeOf<FocusChangedEventArgs>());
        Assert.That(((FocusChangedEventArgs)result!).Focused, Is.True);
    }

    [Test]
    public void HandleInput_FocusLost_ReturnsFocusedFalse()
    {
        EventArgs? result = _provider.HandleInput("\x1b[O");
        Assert.That(result, Is.TypeOf<FocusChangedEventArgs>());
        Assert.That(((FocusChangedEventArgs)result!).Focused, Is.False);
    }

    [Test]
    public void HandleInput_UnknownSequence_ReturnsNull()
    {
        Assert.That(_provider.HandleInput("\x1b[X"), Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(_provider.HandleInput(string.Empty), Is.Null);
    }
}
