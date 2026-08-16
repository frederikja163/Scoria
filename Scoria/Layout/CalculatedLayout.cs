namespace Scoria.Layout;

public sealed class CalculatedLayout
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    internal void SetProperty(Property property, int value)
    {
        switch (property)
        {
            case Property.X:
                X = value;
                break;
            case Property.Y:
                Y = value;
                break;
            case Property.Width:
                Width = value;
                break;
            case Property.Height:
                Height = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}