using NUnit.Framework;
using Scoria;
using Scoria.Layout;

namespace Scoria.Tests.Layout;

using EdgeMap = Dictionary<Reference, HashSet<Reference>>;

[TestFixture]
public class PosTests
{
    private static readonly LayoutSolver DummySolver = new([], []);

    [Test]
    public void Abs_GetReferences_ReturnsEmpty()
    {
        Element self = NewElement();

        List<Reference> references = Pos.Abs(10).GetReferences(Property.X, self);

        Assert.That(references, Is.Empty);
    }

    [Test]
    public void Abs_Solve_ReturnsValue()
    {
        Assert.That(Pos.Abs(42).Solve(DummySolver, []), Is.EqualTo(42));
    }

    [Test]
    public void Abs_Solve_NegativeValue_ReturnsNegative()
    {
        Assert.That(Pos.Abs(-5).Solve(DummySolver, []), Is.EqualTo(-5));
    }

    [Test]
    public void Abs_Solve_IgnoresDependencies()
    {
        Assert.That(Pos.Abs(7).Solve(DummySolver, [999, -5, 3]), Is.EqualTo(7));
    }

    [Test]
    public void Relative_GetReferences_XAxis_ReferencesSelfSizeThenReferencePositionThenReferenceSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<Reference> references = Pos.Relative(0.5f, reference).GetReferences(Property.X, self);

        Assert.That(references, Is.EqualTo(new[]
        {
            new Reference(Property.Width, self),
            new Reference(Property.X, reference),
            new Reference(Property.Width, reference),
        }));
    }

    [Test]
    public void Relative_GetReferences_YAxis_ReferencesSelfHeightThenReferenceYThenReferenceHeight()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<Reference> references = Pos.Relative(0.5f, reference).GetReferences(Property.Y, self);

        Assert.That(references, Is.EqualTo(new[]
        {
            new Reference(Property.Height, self),
            new Reference(Property.Y, reference),
            new Reference(Property.Height, reference),
        }));
    }

    [Test]
    public void Relative_GetReferences_UsesParentWhenNoElementSpecified()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        List<Reference> references = Pos.Relative(0.5f).GetReferences(Property.X, self);

        Assert.That(references, Is.EqualTo(new[]
        {
            new Reference(Property.Width, self),
            new Reference(Property.X, parent),
            new Reference(Property.Width, parent),
        }));
    }

    [Test]
    public void Relative_GetReferences_WithoutParentOrElement_Throws()
    {
        Element self = NewElement();

        Assert.That(
            () => Pos.Relative(0.5f).GetReferences(Property.X, self),
            Throws.Exception.With.Message.EqualTo("Relative position must either specify an element or have a parent element."));
    }

    [Test]
    public void Relative_Solve_ZeroPercent_AlignsToReferencePosition()
    {
        Assert.That(Pos.Relative(0f).Solve(DummySolver, [20, 50, 30]), Is.EqualTo(50));
    }

    [Test]
    public void Relative_Solve_HalfPercent_OffsetsByHalfOfRemainingSpace()
    {
        Assert.That(Pos.Relative(0.5f).Solve(DummySolver, [20, 50, 30]), Is.EqualTo(55));
    }

    [Test]
    public void Relative_Solve_HundredPercent_AlignsToReferenceEnd()
    {
        Assert.That(Pos.Relative(1f).Solve(DummySolver, [20, 50, 30]), Is.EqualTo(60));
    }

    [Test]
    public void Relative_Solve_TruncatesFractionalResultTowardsZero()
    {
        Assert.That(Pos.Relative(0.49f).Solve(DummySolver, [20, 50, 30]), Is.EqualTo(54));
    }

    [Test]
    public void Relative_Solve_NegativeRemainingSpace_Works()
    {
        Assert.That(Pos.Relative(0.5f).Solve(DummySolver, [20, 50, 10]), Is.EqualTo(45));
    }

    [Test]
    public void Center_Solve_CentersWithinReference()
    {
        Assert.That(Pos.Center().Solve(DummySolver, [20, 0, 100]), Is.EqualTo(40));
    }

    [Test]
    public void Center_Solve_OffsetsByReferencePosition()
    {
        Assert.That(Pos.Center().Solve(DummySolver, [20, 10, 100]), Is.EqualTo(50));
    }

    [Test]
    public void Center_Solve_RoundsDownForOddRemainder()
    {
        Assert.That(Pos.Center().Solve(DummySolver, [20, 0, 101]), Is.EqualTo(40));
    }

    [Test]
    public void Solve_AbsoluteChild_ResolvesToGivenValues()
    {
        Element parent = NewElement();
        Element child = NewElement();
        parent.AddChild(child);
        child.X = Pos.Abs(11);
        child.Y = Pos.Abs(22);
        child.Width = new AbsSize(33);
        child.Height = new AbsSize(44);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(solver.GetValue(new Reference(Property.X, child)), Is.EqualTo(11));
        Assert.That(solver.GetValue(new Reference(Property.Y, child)), Is.EqualTo(22));
        Assert.That(solver.GetValue(new Reference(Property.Width, child)), Is.EqualTo(33));
        Assert.That(solver.GetValue(new Reference(Property.Height, child)), Is.EqualTo(44));
    }

    [Test]
    public void Solve_CenteredChild_ResolvesToCenterOfParent()
    {
        Element parent = NewElement(width: 100, height: 80);
        Element child = NewElement();
        parent.AddChild(child);
        child.X = Pos.Center();
        child.Y = Pos.Center();
        child.Width = new AbsSize(20);
        child.Height = new AbsSize(10);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(solver.GetValue(new Reference(Property.X, child)), Is.EqualTo(40));
        Assert.That(solver.GetValue(new Reference(Property.Y, child)), Is.EqualTo(35));
    }

    [Test]
    public void Solve_ChildRelativeToSibling_ResolvesUsingSiblingPositionAndSize()
    {
        Element parent = NewElement();
        Element sibling = NewElement();
        Element child = NewElement();
        parent.AddChild(sibling);
        parent.AddChild(child);

        sibling.X = Pos.Abs(50);
        sibling.Y = Pos.Abs(10);
        sibling.Width = new AbsSize(30);
        sibling.Height = new AbsSize(20);

        child.X = Pos.Relative(0.5f, sibling);
        child.Y = Pos.Relative(0.5f, sibling);
        child.Width = new AbsSize(4);
        child.Height = new AbsSize(4);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(solver.GetValue(new Reference(Property.X, child)), Is.EqualTo(63));
        Assert.That(solver.GetValue(new Reference(Property.Y, child)), Is.EqualTo(18));
    }

    private static LayoutSolver BuildSolver(Element root)
    {
        EdgeMap forwardEdges = [];
        EdgeMap backwardEdges = [];
        Queue<Reference> emptyQueue = [];
        Dictionary<Reference, List<Reference>> dependencies = [];
        foreach (Element element in GetElementsRecursive(root))
        {
            AddEdges(element.X.GetReferences(Property.X, element), new Reference(Property.X, element));
            AddEdges(element.Y.GetReferences(Property.Y, element), new Reference(Property.Y, element));
            AddEdges(element.Width.GetReferences(Property.Width, element), new Reference(Property.Width, element));
            AddEdges(element.Height.GetReferences(Property.Height, element), new Reference(Property.Height, element));
        }

        List<Reference> topologicalOrder = LayoutSolver.TopologicalSort(emptyQueue, forwardEdges, backwardEdges);
        return new LayoutSolver(topologicalOrder, dependencies);

        void AddEdges(List<Reference> targets, Reference source)
        {
            dependencies.Add(source, targets);
            if (!targets.Any())
            {
                emptyQueue.Enqueue(source);
            }
            foreach (Reference target in targets)
            {
                AddEdge(forwardEdges, source, target);
                AddEdge(backwardEdges, target, source);
            }
        }
    }

    private static void AddEdge(EdgeMap edges, Reference source, Reference target)
    {
        if (!edges.TryGetValue(source, out HashSet<Reference>? targets))
        {
            targets = [];
            edges.Add(source, targets);
        }
        targets.Add(target);
    }

    private static IEnumerable<Element> GetElementsRecursive(Element element)
    {
        yield return element;
        foreach (Element child in element.GetChildren())
        {
            foreach (Element nested in GetElementsRecursive(child))
            {
                yield return nested;
            }
        }
    }

    private sealed class AbsSize(int value) : Size
    {
        internal override List<Reference> GetReferences(Property property, Element self) => [];
        internal override int Solve(LayoutSolver solver, List<int> dependencies) => value;
    }

    private static Element NewElement(int width = 0, int height = 0)
    {
        return new PanelElement
        {
            Title = "element",
            X = Pos.Abs(0),
            Y = Pos.Abs(0),
            Width = new AbsSize(width),
            Height = new AbsSize(height),
        };
    }
}
