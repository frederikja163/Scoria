namespace Scoria.Events;

public sealed class KeyEventArgs : AnyEventArgs
{
    public Key Key { get; }
    public char? Char { get; }

    internal KeyEventArgs(Key key, char? c)
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