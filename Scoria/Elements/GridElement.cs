using System.Diagnostics;
using Scoria.Layout;

namespace Scoria.Elements;

public enum GridFillMethod
{
    ColumnThenRow,
    RowThenColumn,
    Explicit,
}
public enum GridResizeMethod
{
    Columns,
    Rows,
    NoResize,
}
// TODO: Consider endpoints like resize(element wise), clear cell, expand element, AddChild(element,4xint) etc.
// There are so many endpoints we can add here, and its mostly just syntactic sugar so its not worth it adding all of them.
public sealed class GridElement : Element
{
    internal sealed class Node : Element
    {
        public new GridElement Parent { get; }
        public Range Columns { get; }
        public Range Rows { get; }
        public Element Element { get; }

        internal Node(GridElement parent, Element element, Range columns, Range rows)
        {
            Parent = parent;
            Columns = columns;
            Rows = rows;
            Element = element;
            AddChild(element);
        }

        protected override bool SupportsChild(Element element) => !GetChildren().Any();

        protected override bool SupportsParent(Element? element) => element is GridElement;

        protected internal override List<LayoutProperty> ResolveAutoLayoutDependencies(LayoutPropertyType type)
        {
            return [new LayoutProperty(type.SameAxisSize(), Parent)];
        }

        protected internal override int ResolveAutoLayout(LayoutPropertyType type, List<int> dependencies)
        {
            (bool position, Range range, int length, int gap) = type switch
            {
                LayoutPropertyType.X => (true, Columns, Parent.Columns, Parent.ColumnGap),
                LayoutPropertyType.Y => (true, Rows, Parent.Rows, Parent.RowGap),
                LayoutPropertyType.Width => (false, Columns, Parent.Columns, Parent.ColumnGap),
                LayoutPropertyType.Height => (false, Rows, Parent.Rows, Parent.RowGap),
                _ => throw new UnreachableException(),
            };
            // TODO: Handle parent size
            (int cellStart, int cellLength) = range.GetOffsetAndLength(length);
            int spacing = dependencies[0] / length;
            return position ? cellStart * spacing : cellLength * spacing - gap;
        }
    }

    private List<Node>[,] _nodes;
    private readonly Dictionary<Element, Node> _nodeMap = new();

    public GridElement(int columns, int rows)
    {
        Columns = Math.Max(1, columns);
        Rows = Math.Max(1, rows);
        _nodes = new List<Node>[Columns, Rows];
        for (int c = 0; c < Columns; c++)
        {
            for (int r = 0; r < Rows; r++)
            {
                _nodes[c, r] = [];
            }
        }
    }

    public int Columns { get; private set; }

    public int Rows { get; private set; }

    public GridFillMethod FillMethod { get; set; } = GridFillMethod.RowThenColumn;

    public GridResizeMethod ResizeMethod { get; set; } = GridResizeMethod.Rows;

    public int ColumnGap
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    }

    public int RowGap
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    }

    public override void AddChild(Element element)
    {
        List<Node> nodes = FindFirstEmpty(out int col, out int row);
        Node node = new(this, element, new(col, col + 1), new(row, row + 1));
        base.AddChild(node);
        _nodeMap.Add(element, node);
        nodes.Add(node);
    }

    public void AddChild(Element element, int col, int row) => AddChild(element, new Range(col, col), new Range(row, row));

    public void AddChild(Element element, Index col, Index row) => AddChild(element, new Range(col, col), new Range(row, row));

    public void AddChild(Element element, Range colRange, Range rowRange)
    {
        (int minCol, int cols) = colRange.GetOffsetAndLength(Columns);
        (int minRow, int rows) = rowRange.GetOffsetAndLength(Rows);

        Node node = new(this, element, colRange, rowRange);
        base.AddChild(node);
        _nodeMap.Add(element, node);
        for (int c = 0; c <= cols; c++)
        {
            for (int r = 0; r <= rows; r++)
            {
                List<Node> nodes = _nodes[c + minCol, r + minRow];
                nodes.Add(node);
            }
        }
    }

    public override void RemoveChild(Element element)
    {
        Node node = GetNode(element);
        _nodeMap.Remove(element);
        base.RemoveChild(node);

        (int minCol, int cols) = node.Columns.GetOffsetAndLength(Columns);
        (int minRow, int rows) = node.Rows.GetOffsetAndLength(Rows);
        for (int c = 0; c <= cols; c++)
        {
            for (int r = 0; r <= rows; r++)
            {
                List<Node> nodes = _nodes[c + minCol, r + minRow];
                nodes.Remove(node);
            }
        }
    }

    private Node GetNode(Element element)
    {
        return _nodeMap.GetValueOrDefault(element) ?? throw new Exception($"{element} is not part of the grid {this}");
    }

    private List<Node> FindFirstEmpty(out int col, out int row)
    {
        switch (FillMethod)
        {
            case GridFillMethod.Explicit:
                throw new InvalidOperationException($"Cannot call the overload {nameof(AddChild)}({nameof(Element)}) on a {nameof(GridElement)} with {nameof(FillMethod)} = {GridFillMethod.Explicit}");
            case GridFillMethod.RowThenColumn:
                for (row = 0; row < Rows; row++)
                {
                    for (col = 0; col < Columns; col++)
                    {
                        if (_nodes[col, row].Count != 0)
                            continue;
                        return _nodes[col, row];
                    }
                }
                break;
            case GridFillMethod.ColumnThenRow:
                for (col = 0; col < Columns; col++)
                {
                    for (row = 0; row < Rows; row++)
                    {
                        if (_nodes[col, row].Count != 0)
                            continue;
                        return _nodes[col, row];
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(FillMethod));
        }
        col = row = 0;
        return []; // TODO: Resize instead.
    }
    // TODO: Resize grid
}
