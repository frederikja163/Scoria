using NUnit.Framework;
using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class KeyInputProviderTests
{
    private KeyInputProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new KeyInputProvider();
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

    [TestCase('a', Key.A)]
    [TestCase('Q', Key.Q | Key.Shift)]
    [TestCase('5', Key.D5)]
    [TestCase(' ', Key.Space)]
    public void HandleInput_SingleChar_ReturnsKeyEventArgs(char c, Key expected)
    {
        EventArgs? result = Handle(_provider, c.ToString());
        Assert.That(result, Is.TypeOf<KeyEventArgs>());
        var args = (KeyEventArgs)result!;
        Assert.That(args.Key, Is.EqualTo(expected));
        Assert.That(args.Char, Is.EqualTo(c));
    }

    [TestCase('a', Key.A | Key.Alt)]
    [TestCase('Q', Key.Q | Key.Alt | Key.Shift)]
    [TestCase('5', Key.D5 | Key.Alt)]
    [TestCase(' ', Key.Space | Key.Alt)]
    public void HandleInput_AltKey_ReturnsAltModifiedKey(char c, Key expected)
    {
        EventArgs? result = Handle(_provider, $"\x1b{c}");
        Assert.That(result, Is.TypeOf<KeyEventArgs>());
        var args = (KeyEventArgs)result!;
        Assert.That(args.Key, Is.EqualTo(expected));
        Assert.That(args.Char, Is.EqualTo(c));
    }

    [TestCase('\x01', Key.A | Key.Ctrl)]
    [TestCase('\x1A', Key.Z | Key.Ctrl)]
    [TestCase('\x03', Key.C | Key.Ctrl)]
    [TestCase('\x00', Key.Space | Key.Ctrl)]
    public void HandleInput_CtrlKey_ReturnsCtrlModifiedKey(char c, Key expected)
    {
        EventArgs? result = Handle(_provider, c.ToString());
        Assert.That(result, Is.TypeOf<KeyEventArgs>());
        var args = (KeyEventArgs)result!;
        Assert.That(args.Key, Is.EqualTo(expected));
        Assert.That(args.Char, Is.Null);
    }

    [TestCase('\x08')]
    [TestCase('\x09')]
    [TestCase('\x0A')]
    [TestCase('\x0D')]
    [TestCase('\x1B')]
    public void HandleInput_CollidingControlChar_KeepsNamedKey(char c)
    {
        EventArgs? result = Handle(_provider, c.ToString());
        var args = (KeyEventArgs)result!;
        Assert.That(args.Key.HasFlag(Key.Ctrl), Is.False);
    }

    [TestCase("\x1b[A", Key.Up)]
    [TestCase("\x1b[B", Key.Down)]
    [TestCase("\x1bOP", Key.F1)]
    [TestCase("\x1b[15~", Key.F5)]
    public void HandleInput_EscapeSequence_ReturnsKeyEventArgs(string input, Key expected)
    {
        EventArgs? result = Handle(_provider, input);
        Assert.That(result, Is.TypeOf<KeyEventArgs>());
        var args = (KeyEventArgs)result!;
        Assert.That(args.Key, Is.EqualTo(expected));
        Assert.That(args.Char, Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(Handle(_provider, string.Empty), Is.Null);
    }

    [Test]
    public void HandleInput_ConsumesMatchedSequence()
    {
        ReadOnlySpan<char> span = "\x1b[A";
        _provider.HandleInput(ref span);
        Assert.That(span.IsEmpty, Is.True);
    }

    [Test]
    public void HandleInput_DoesNotConsumeUnmatchedSequence()
    {
        ReadOnlySpan<char> span = "xy";
        _provider.HandleInput(ref span);
        Assert.That(span.ToString(), Is.EqualTo("xy"));
    }
}
