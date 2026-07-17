using NUnit.Framework;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class PasteInputProviderTests
{
    private PasteInputProvider _provider = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new PasteInputProvider();
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
    public void HandleInput_ValidPaste_ReturnsPasteEventArgs()
    {
        EventArgs? result = _provider.HandleInput("\x1b[200~hello world\x1b[201~");
        Assert.That(result, Is.TypeOf<PasteEventArgs>());
        Assert.That(((PasteEventArgs)result!).Text, Is.EqualTo("hello world"));
    }

    [Test]
    public void HandleInput_EmptyPaste_ReturnsPasteEventArgsWithEmptyText()
    {
        EventArgs? result = _provider.HandleInput("\x1b[200~\x1b[201~");
        Assert.That(result, Is.TypeOf<PasteEventArgs>());
        Assert.That(((PasteEventArgs)result!).Text, Is.EqualTo(string.Empty));
    }

    [Test]
    public void HandleInput_MissingStart_ReturnsNull()
    {
        Assert.That(_provider.HandleInput("hello\x1b[201~"), Is.Null);
    }

    [Test]
    public void HandleInput_MissingStop_ReturnsNull()
    {
        Assert.That(_provider.HandleInput("\x1b[200~hello"), Is.Null);
    }

    [Test]
    public void HandleInput_PlainText_ReturnsNull()
    {
        Assert.That(_provider.HandleInput("just some text"), Is.Null);
    }

    [Test]
    public void HandleInput_EmptyString_ReturnsNull()
    {
        Assert.That(_provider.HandleInput(string.Empty), Is.Null);
    }
}
