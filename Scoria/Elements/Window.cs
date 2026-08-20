namespace Scoria.Elements;

public sealed class Window : Element
{
    protected override bool SupportsParent(Element? parent)
    {
        return false;
    }
}