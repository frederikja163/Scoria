using NUnit.Framework;
using Scoria.Drivers;
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

    private static EventArgs? Handle(IInputProvider provider, string input)
    {
        ReadOnlySpan<char> span = input;
        return provider.HandleInput(ref span);
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
        EventArgs? result = Handle(_provider, "\x1b[I");
        Assert.That(result, Is.TypeOf<FocusChangedEventArgs>());
        Assert.That(((FocusChangedEventArgs)result!).Focused, Is.True);
    }

    [Test]
    public void HandleInput_FocusLost_ReturnsFocusedFalse()
    {
        EventArgs? result = Handle(_provider, "\x1b[O");
        Assert.That(result, Is.TypeOf<FocusChangedEventArgs>());
        Assert.That(((FocusChangedEventArgs)result!).Focused, Is.False);
    }

    [Test]
    public void HandleInput_UnknownSequence_ReturnsNull()
    {
        Assert.That(Handle(_provider, "\x1b[X"), Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(Handle(_provider, string.Empty), Is.Null);
    }

    [Test]
    public void HandleInput_ConsumesMatchedSequence()
    {
        ReadOnlySpan<char> span = "\x1b[I";
        _provider.HandleInput(ref span);
        Assert.That(span.IsEmpty, Is.True);
    }

    [Test]
    public void HandleInput_DoesNotConsumeUnmatchedSequence()
    {
        ReadOnlySpan<char> span = "\x1b[X";
        _provider.HandleInput(ref span);
        Assert.That(span.ToString(), Is.EqualTo("\x1b[X"));
    }
}
