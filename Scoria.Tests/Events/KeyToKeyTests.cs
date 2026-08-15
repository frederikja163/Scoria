using NUnit.Framework;
using Scoria.Events;

namespace Scoria.Tests.Events;

[TestFixture]
public class KeyToKeyTests
{
    [TestCase('a', Key.A)]
    [TestCase('z', Key.Z)]
    [TestCase('m', Key.M)]
    public void LowercaseLetter_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.FromChar(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [TestCase('A', Key.A | Key.Shift)]
    [TestCase('Z', Key.Z | Key.Shift)]
    [TestCase('M', Key.M | Key.Shift)]
    public void UppercaseLetter_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.FromChar(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [TestCase('0', Key.D0)]
    [TestCase('5', Key.D5)]
    [TestCase('9', Key.D9)]
    public void Digit_MapsCorrectly(char c, Key expected)
    {
        Key key = Key.FromChar(c, out var ch);
        Assert.That(key, Is.EqualTo(expected));
        Assert.That(ch, Is.EqualTo(c));
    }

    [Test]
    public void Space_MapsToSpace()
    {
        Key key = Key.FromChar(' ', out var ch);
        Assert.That(key, Is.EqualTo(Key.Space));
        Assert.That(ch, Is.EqualTo(' '));
    }

    [TestCase('\r')]
    [TestCase('\n')]
    public void CarriageReturnAndNewline_MapsToEnter(char c)
    {
        Key key = Key.FromChar(c, out var ch);
        Assert.That(key, Is.EqualTo(Key.Enter));
        Assert.That(ch, Is.EqualTo('\n'));
    }

    [Test]
    public void Tab_MapsToTab()
    {
        Key key = Key.FromChar('\t', out var ch);
        Assert.That(key, Is.EqualTo(Key.Tab));
        Assert.That(ch, Is.EqualTo('\t'));
    }

    [Test]
    public void Escape_MapsToEscapeWithNullChar()
    {
        Key key = Key.FromChar('\x1B', out var ch);
        Assert.That(key, Is.EqualTo(Key.Escape));
        Assert.That(ch, Is.Null);
    }

    [Test]
    public void BackspaceAscii_MapsToBackspaceWithNullChar()
    {
        Key key = Key.FromChar('\x7F', out var ch);
        Assert.That(key, Is.EqualTo(Key.Backspace));
        Assert.That(ch, Is.Null);
    }

    [Test]
    public void BackspaceControl_MapsToBackspaceWithNullChar()
    {
        Key key = Key.FromChar('\x08', out var ch);
        Assert.That(key, Is.EqualTo(Key.Backspace));
        Assert.That(ch, Is.Null);
    }

    [TestCase('!')]
    [TestCase('@')]
    [TestCase('ñ')]
    public void UnmappedCharacter_MapsToUnicode(char c)
    {
        Key key = Key.FromChar(c, out var ch);
        Assert.That(key, Is.EqualTo(Key.Unicode));
        Assert.That(ch, Is.EqualTo(c));
    }

    [Test]
    public void CaseVariants_ShareSameBaseKey()
    {
        Key lower = Key.FromChar('a', out _);
        Key upper = Key.FromChar('A', out _);
        Assert.That(upper, Is.EqualTo(lower | Key.Shift));
        Assert.That(upper.Base(), Is.EqualTo(lower));
    }
}
