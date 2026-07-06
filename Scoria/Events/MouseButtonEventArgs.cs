namespace Scoria.Drivers;

public sealed class MouseButtonEventArgs : EventArgs
{
    internal MouseButtonEventArgs(Button button, int x, int y, bool down)
    {
        Button = button;
        X = x;
        Y = y;
        Down = down;
    }

    public Button Button { get; }
    public int X { get; }
    public int Y { get; }
    public bool Down { get; }

    public override string ToString()
    {
        string down = Down ? "down" : "up";
        return $"Mouse {Button} {down} ({X}, {Y})";
    }
}