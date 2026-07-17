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

[TestFixture]
public class KeyToKeyTests
{
    [TestCase('a', Key.A)]
    [TestCase('z', Key.Z)]
    [TestCase('m', Key.M)]
    public void LowercaseLetter_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.ToKey(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [TestCase('A', Key.A)]
    [TestCase('Z', Key.Z)]
    [TestCase('M', Key.M)]
    public void UppercaseLetter_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.ToKey(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [TestCase('0', Key.D0)]
    [TestCase('5', Key.D5)]
    [TestCase('9', Key.D9)]
    public void Digit_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.ToKey(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [Test]
    public void Space_MapsToSpace()
    {
        Key key = Key.ToKey(' ', out var ch);
        Assert.That(key, Is.EqualTo(Key.Space));
        Assert.That(ch, Is.EqualTo(' '));
    }

    [TestCase('\r')]
    [TestCase('\n')]
    public void CarriageReturnAndNewline_MapsToEnter(char c)
    {
        Key key = Key.ToKey(c, out var ch);
        Assert.That(key, Is.EqualTo(Key.Enter));
        Assert.That(ch, Is.EqualTo('\n'));
    }

    [Test]
    public void Tab_MapsToTab()
    {
        Key key = Key.ToKey('\t', out var ch);
        Assert.That(key, Is.EqualTo(Key.Tab));
        Assert.That(ch, Is.EqualTo('\t'));
    }

    [Test]
    public void Escape_MapsToEscapeWithNullChar()
    {
        Key key = Key.ToKey('\x1B', out var ch);
        Assert.That(key, Is.EqualTo(Key.Escape));
        Assert.That(ch, Is.Null);
    }

    [Test]
    public void BackspaceAscii_MapsToBackspaceWithNullChar()
    {
        Key key = Key.ToKey('\x7F', out var ch);
        Assert.That(key, Is.EqualTo(Key.Backspace));
        Assert.That(ch, Is.Null);
    }

    [Test]
    public void BackspaceControl_MapsToBackspaceWithNullChar()
    {
        Key key = Key.ToKey('\x08', out var ch);
        Assert.That(key, Is.EqualTo(Key.Backspace));
        Assert.That(ch, Is.Null);
    }

    [TestCase('!')]
    [TestCase('@')]
    [TestCase('ñ')]
    public void UnmappedCharacter_MapsToUnicode(char c)
    {
        Key key = Key.ToKey(c, out var ch);
        Assert.That(key, Is.EqualTo(Key.Unicode));
        Assert.That(ch, Is.EqualTo(c));
    }

    [Test]
    public void CaseInsensitiveLetter_ProducesSameKey()
    {
        Key lower = Key.ToKey('a', out _);
        Key upper = Key.ToKey('A', out _);
        Assert.That(lower, Is.EqualTo(upper));
    }
}
