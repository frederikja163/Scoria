namespace Scoria.Events;

public sealed class KeyEventArgs : EventArgs
{
    public Key Key { get; }
    public char? Char { get; }

    public KeyEventArgs(Key key, char? c)
    {
        Key = key;
        Char = c;
    }

    public override string ToString()
    {
        string charString = Char is null ? string.Empty : $"({Char})";
        return $"Key {Key} {charString}";
    }
}