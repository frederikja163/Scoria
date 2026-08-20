using NUnit.Framework;
using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class ResizeInputProviderTests
{
    private ResizeInputProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new ResizeInputProvider();
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

    [TestCase("\x1b[48;24;80;240;1600t", 80, 24)]
    [TestCase("\x1b[8;24;80t", 80, 24)]
    [TestCase("\x1b[48;34;126t", 126, 34)]
    public void HandleInput_ResizeReport_ReturnsResizeEventArgs(string input, int width, int height)
    {
        EventArgs? result = Handle(_provider, input);
        Assert.That(result, Is.TypeOf<WindowResizeEventArgs>());
        var args = (WindowResizeEventArgs)result!;
        Assert.That(args.Width, Is.EqualTo(width));
        Assert.That(args.Height, Is.EqualTo(height));
    }

    [Test]
    public void HandleInput_InvalidInput_ReturnsNull()
    {
        Assert.That(Handle(_provider, "not a resize event"), Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(Handle(_provider, string.Empty), Is.Null);
    }

    [Test]
    public void HandleInput_ConsumesMatchedSequence()
    {
        ReadOnlySpan<char> span = "\x1b[48;24;80;240;1600t";
        _provider.HandleInput(ref span);
        Assert.That(span.IsEmpty, Is.True);
    }

    [Test]
    public void HandleInput_LeavesTrailingDataAfterResize()
    {
        ReadOnlySpan<char> span = "\x1b[48;24;80;240;1600ttrailing";
        _provider.HandleInput(ref span);
        Assert.That(span.ToString(), Is.EqualTo("trailing"));
    }

    [Test]
    public void HandleInput_DoesNotConsumeUnmatchedSequence()
    {
        ReadOnlySpan<char> span = "not a resize event";
        _provider.HandleInput(ref span);
        Assert.That(span.ToString(), Is.EqualTo("not a resize event"));
    }
}
