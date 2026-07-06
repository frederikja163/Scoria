namespace Scoria.Drivers;

public sealed class MouseScrollEventArgs : EventArgs
{
    internal MouseScrollEventArgs(int x, int y, bool down)
    {
        X = x;
        Y = y;
        Down = down;
    }
    
    public int X { get; }
    public int Y { get; }
    public bool Down { get; }

    public override string ToString()
    {
        string down = Down ? "down" : "up";
        return $"Mouse scroll {down}";
    }
}