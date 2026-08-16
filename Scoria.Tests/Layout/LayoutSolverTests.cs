using NUnit.Framework;
using Scoria;
using Scoria.Layout;

namespace Scoria.Tests.Layout;

using EdgeMap = Dictionary<Reference, HashSet<Reference>>;

[TestFixture]
public class LayoutSolverTests
{
    [Test]
    public void TopologicalSort_NoNodes_ReturnsEmptyOrder()
    {
        List<Reference> order = RunTopologicalSort(BuildGraph([]));

        Assert.That(order, Is.Empty);
    }

    [Test]
    public void TopologicalSort_SingleDependency_TargetSolvedBeforeSource()
    {
        Reference source = Ref();
        Reference target = Ref();

        List<Reference> order = RunTopologicalSort(BuildGraph([source, target], (source, target)));

        Assert.That(order, Is.EquivalentTo(new[] { source, target }));
        Assert.That(order.IndexOf(target), Is.LessThan(order.IndexOf(source)));
    }

    [Test]
    public void TopologicalSort_LinearChain_DependenciesBeforeDependents()
    {
        Reference a = Ref();
        Reference b = Ref();
        Reference c = Ref();

        List<Reference> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b), (b, c)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c }));
        Assert.That(order.IndexOf(c), Is.LessThan(order.IndexOf(b)));
        Assert.That(order.IndexOf(b), Is.LessThan(order.IndexOf(a)));
    }

    [Test]
    public void TopologicalSort_Diamond_RespectsAllDependencies()
    {
        Reference a = Ref();
        Reference b = Ref();
        Reference c = Ref();
        Reference d = Ref();

        List<Reference> order = RunTopologicalSort(BuildGraph([a, b, c, d], (a, b), (a, c), (b, d), (c, d)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c, d }));
        Assert.That(order.IndexOf(d), Is.LessThan(order.IndexOf(b)));
        Assert.That(order.IndexOf(d), Is.LessThan(order.IndexOf(c)));
        Assert.That(order.IndexOf(b), Is.LessThan(order.IndexOf(a)));
        Assert.That(order.IndexOf(c), Is.LessThan(order.IndexOf(a)));
    }

    [Test]
    public void TopologicalSort_NodeWithoutDependencies_IsIncluded()
    {
        Reference a = Ref();
        Reference b = Ref();
        Reference c = Ref();

        List<Reference> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c }));
    }

    [Test]
    public void TopologicalSort_EveryNodeAppearsExactlyOnce()
    {
        Reference a = Ref();
        Reference b = Ref();
        Reference c = Ref();

        List<Reference> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b), (b, c)));

        Assert.That(order, Has.Exactly(1).EqualTo(a));
        Assert.That(order, Has.Exactly(1).EqualTo(b));
        Assert.That(order, Has.Exactly(1).EqualTo(c));
    }

    [Test]
    public void TopologicalSort_SelfLoop_Throws()
    {
        Reference a = Ref();

        Assert.That(
            () => RunTopologicalSort(BuildGraph([a], (a, a))),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    [Test]
    public void TopologicalSort_MutualCycle_Throws()
    {
        Reference a = Ref();
        Reference b = Ref();

        Assert.That(
            () => RunTopologicalSort(BuildGraph([a, b], (a, b), (b, a))),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    [Test]
    public void TopologicalSort_ThreeNodeCycle_Throws()
    {
        Reference a = Ref();
        Reference b = Ref();
        Reference c = Ref();

        Assert.That(
            () => RunTopologicalSort(BuildGraph([a, b, c], (a, b), (b, c), (c, a))),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    [Test]
    public void Solve_ElementWithoutDependencies_DoesNotThrow()
    {
        Element element = NewElement();

        Assert.DoesNotThrow(() => LayoutSolver.Solve(element));
    }

    [Test]
    public void Solve_DependentElements_DoesNotThrow()
    {
        Element parent = NewElement();
        Element a = NewElement();
        Element b = NewElement();
        parent.AddChild(a);
        parent.AddChild(b);
        ((TestPos)b.X).References.Add(new Reference(Property.X, a));

        Assert.DoesNotThrow(() => LayoutSolver.Solve(parent));
    }

    [Test]
    public void Solve_CyclicDependencies_Throws()
    {
        Element parent = NewElement();
        Element a = NewElement();
        Element b = NewElement();
        parent.AddChild(a);
        parent.AddChild(b);
        ((TestPos)a.X).References.Add(new Reference(Property.X, b));
        ((TestPos)b.X).References.Add(new Reference(Property.X, a));

        Assert.That(
            () => LayoutSolver.Solve(parent),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    private static Reference Ref()
    {
        return new Reference(Property.X, new PanelElement { Title = "ref" });
    }

    private static (Queue<Reference> Queue, EdgeMap Forward, EdgeMap Backward) BuildGraph(
        Reference[] nodes,
        params (Reference Source, Reference Target)[] edges)
    {
        Queue<Reference> queue = new();
        EdgeMap forward = [];
        EdgeMap backward = [];
        HashSet<Reference> sources = [];

        foreach ((Reference source, Reference target) in edges)
        {
            AddEdge(forward, source, target);
            AddEdge(backward, target, source);
            sources.Add(source);
        }

        foreach (Reference node in nodes)
        {
            if (!sources.Contains(node))
            {
                queue.Enqueue(node);
            }
        }

        return (queue, forward, backward);
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

    private static List<Reference> RunTopologicalSort(
        (Queue<Reference> Queue, EdgeMap Forward, EdgeMap Backward) graph)
    {
        return LayoutSolver.TopologicalSort(graph.Queue, graph.Forward, graph.Backward);
    }

    private sealed class TestPos : Pos
    {
        public List<Reference> References { get; } = [];
        internal override List<Reference> GetReferences(Property property, Element self) => References;
        internal override int Solve(LayoutSolver solver, List<int> dependencies) => 0;
    }

    private sealed class TestSize : Size
    {
        internal override List<Reference> GetReferences(Property property, Element self) => [];
        internal override int Solve(LayoutSolver solver, List<int> dependencies) => 0;
    }

    private static Element NewElement()
    {
        return new PanelElement
        {
            Title = "element",
            X = new TestPos(),
            Y = new TestPos(),
            Width = new TestSize(),
            Height = new TestSize(),
        };
    }
}
