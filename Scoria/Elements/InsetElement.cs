using Scoria;
using Scoria.Elements;
using Scoria.Layout;

public sealed class InsetElement : Element
{
    private sealed class Node : Element
    {
        private readonly InsetElement _element;
        public Node(InsetElement element)
        {
            _element = element;
            XDefault = Pos.Begin();
            YDefault = Pos.Begin();
            WidthDefault = Size.Fill();
            HeightDefault = Size.Fill();
        }

        protected internal override int ResolveAutoLayout(LayoutPropertyType type, ref ReadOnlySpan<int> dependencies)
        {
            int auto = base.ResolveAutoLayout(type, ref dependencies);
            return type switch
            {
                LayoutPropertyType.X => auto + _element.LeftInset,
                LayoutPropertyType.Y => auto + _element.TopInset,
                LayoutPropertyType.Width => auto - _element.LeftInset - _element.RightInset,
                LayoutPropertyType.Height => auto - _element.TopInset - _element.BottomInset,
                _ => auto,
            };
        }

        protected override bool SupportsParent(Element? parent)
        {
            return parent is InsetElement;
        }

        protected override void Render(ISurface surface)
        {
            surface.SubSurface(CalculatedLayout).Fill(' ', _element.InnerStyle);
            base.Render(surface);
        }
    }
    private readonly Node _node;

    public InsetElement()
    {
        _node = new(this);
        base.AddChild(_node);
    }

    public InsetElement(int inset) : this(inset, inset) { }

    public InsetElement(int xInset, int yInset) : this(xInset, yInset, xInset, yInset) { }

    public InsetElement(int rightInset, int bottomInset, int leftInset, int topInset) : this()
    {
        RightInset = rightInset;
        BottomInset = bottomInset;
        LeftInset = leftInset;
        TopInset = topInset;
    }

    public Style OuterStyle { get; set { field = value; SetNeedsRender(); } }
    public Style InnerStyle { get; set { field = value; SetNeedsRender(); } }

    public int LeftInset
    {
        get; set { field = value; SetNeedsCalculateLayout(); }
    }
    public int RightInset
    {
        get; set { field = value; SetNeedsCalculateLayout(); }
    }
    public int TopInset
    {
        get; set { field = value; SetNeedsCalculateLayout(); }
    }
    public int BottomInset
    {
        get; set { field = value; SetNeedsCalculateLayout(); }
    }

    public override void AddChild(Element element)
    {
        _node.AddChild(element);
    }

    protected override void Render(ISurface surface)
    {
        surface.SubSurface(CalculatedLayout).Fill(' ', OuterStyle);
        base.Render(surface);
    }
}
