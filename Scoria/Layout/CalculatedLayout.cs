namespace Scoria.Layout;

public sealed class CalculatedLayout
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    internal void SetProperty(LayoutPropertyType layoutPropertyType, int value)
    {
        switch (layoutPropertyType)
        {
            case LayoutPropertyType.X:
                X = value;
                break;
            case LayoutPropertyType.Y:
                Y = value;
                break;
            case LayoutPropertyType.Width:
                Width = value;
                break;
            case LayoutPropertyType.Height:
                Height = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}