namespace Scoria.Events;

/// <summary>Provides data for keyboard input events.</summary>
public sealed class KeyEventArgs : AnyEventArgs
{
    /// <summary>Gets the logical key associated with this event.</summary>
    public Key Key { get; }
    /// <summary>Gets the character produced by the key press, or <see langword="null"/> for non-character keys.</summary>
    public char? Char { get; }

    internal KeyEventArgs(Key key, char? c)
    {
        Key = key;
        Char = c;
    }

    /// <summary>Returns a string representation of the key event, e.g. "Key A (a)".</summary>
    public override string ToString()
    {
        string charString = Char is null ? string.Empty : $"({Char})";
        return $"Key {Key} {charString}";
    }
}