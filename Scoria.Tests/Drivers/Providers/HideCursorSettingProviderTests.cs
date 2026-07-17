using NUnit.Framework;
using Scoria.Drivers.Providers;

namespace Scoria.Tests.Drivers.Providers;

[TestFixture]
public class HideCursorSettingProviderTests
{
    [Test]
    public void Enable_ReturnsTrue()
    {
        var provider = new HideCursorSettingProvider();
        Assert.That(provider.Enable, Is.True);
    }

    [Test]
    public void Order_ReturnsZero()
    {
        var provider = new HideCursorSettingProvider();
        Assert.That(provider.Order, Is.EqualTo(0));
    }
}
