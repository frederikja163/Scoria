namespace Scoria.Drivers;

public sealed class MouseMoveEventArgs : EventArgs
{
    internal MouseMoveEventArgs(int x, int y, int prevX, int prevY)
    {
        X = x;
        Y = y;
        PrevX = prevX;
        PrevY = prevY;
    }

    public int X { get; }
    public int Y { get; }
    public int PrevX { get; }
    public int PrevY { get; }
    public int DeltaX => X - PrevX;
    public int DeltaY => Y - PrevY;

    public override string ToString()
    {
        return $"Mouse move ({PrevX}, {PrevY}) -> ({X}, {Y})";
    }
}