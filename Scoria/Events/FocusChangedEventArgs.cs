namespace Scoria.Drivers;

public sealed class FocusChangedEventArgs : EventArgs
{
    internal FocusChangedEventArgs(bool focused)
    {
        Focused = focused;
    }

    public bool Focused { get; }

    public override string ToString()
    {
        string focused = Focused ? "Gained" : "Lost";
        return $"Focus {focused}";
    }
}