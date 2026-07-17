using NUnit.Framework;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class MouseInputProviderTests
{
    private MouseInputProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new MouseInputProvider();
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
    public void HandleInput_MouseMove_ReturnsMouseMoveEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<32;10;20M");
        Assert.That(result, Is.TypeOf<MouseMoveEventArgs>());
        var args = (MouseMoveEventArgs)result!;
        Assert.That(args.X, Is.EqualTo(10));
        Assert.That(args.Y, Is.EqualTo(20));
    }

    [Test]
    public void HandleInput_LeftButtonDown_ReturnsMouseButtonEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<0;5;15M");
        Assert.That(result, Is.TypeOf<MouseButtonEventArgs>());
        var args = (MouseButtonEventArgs)result!;
        Assert.That(args.Button, Is.EqualTo(Button.Left));
        Assert.That(args.Down, Is.True);
    }

    [Test]
    public void HandleInput_RightButtonDown_ReturnsMouseButtonEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<2;5;15M");
        Assert.That(result, Is.TypeOf<MouseButtonEventArgs>());
        var args = (MouseButtonEventArgs)result!;
        Assert.That(args.Button, Is.EqualTo(Button.Right));
        Assert.That(args.Down, Is.True);
    }

    [Test]
    public void HandleInput_MiddleButtonDown_ReturnsMouseButtonEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<1;5;15M");
        Assert.That(result, Is.TypeOf<MouseButtonEventArgs>());
        var args = (MouseButtonEventArgs)result!;
        Assert.That(args.Button, Is.EqualTo(Button.Middle));
        Assert.That(args.Down, Is.True);
    }

    [Test]
    public void HandleInput_ButtonUp_ReturnsDownFalse()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<0;5;15m");
        Assert.That(result, Is.TypeOf<MouseButtonEventArgs>());
        var args = (MouseButtonEventArgs)result!;
        Assert.That(args.Down, Is.False);
    }

    [Test]
    public void HandleInput_ScrollUp_ReturnsMouseScrollEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<64;5;15M");
        Assert.That(result, Is.TypeOf<MouseScrollEventArgs>());
        var args = (MouseScrollEventArgs)result!;
        Assert.That(args.Down, Is.False);
    }

    [Test]
    public void HandleInput_ScrollDown_ReturnsMouseScrollEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[<65;5;15M");
        Assert.That(result, Is.TypeOf<MouseScrollEventArgs>());
        var args = (MouseScrollEventArgs)result!;
        Assert.That(args.Down, Is.True);
    }

    [Test]
    public void HandleInput_InvalidInput_ReturnsNull()
    {
        Assert.That(_provider.HandleInput("not a mouse event"), Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(_provider.HandleInput(string.Empty), Is.Null);
    }
}
