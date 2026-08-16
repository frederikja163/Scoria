using NUnit.Framework;
using Scoria;
using Scoria.Layout;

namespace Scoria.Tests.Layout;

using EdgeMap = Dictionary<LayoutProperty, HashSet<LayoutProperty>>;

[TestFixture]
public class LayoutSolverTests
{
    [Test]
    public void TopologicalSort_NoNodes_ReturnsEmptyOrder()
    {
        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([]));

        Assert.That(order, Is.Empty);
    }

    [Test]
    public void TopologicalSort_SingleDependency_TargetSolvedBeforeSource()
    {
        LayoutProperty source = Ref();
        LayoutProperty target = Ref();

        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([source, target], (source, target)));

        Assert.That(order, Is.EquivalentTo(new[] { source, target }));
        Assert.That(order.IndexOf(target), Is.LessThan(order.IndexOf(source)));
    }

    [Test]
    public void TopologicalSort_LinearChain_DependenciesBeforeDependents()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();
        LayoutProperty c = Ref();

        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b), (b, c)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c }));
        Assert.That(order.IndexOf(c), Is.LessThan(order.IndexOf(b)));
        Assert.That(order.IndexOf(b), Is.LessThan(order.IndexOf(a)));
    }

    [Test]
    public void TopologicalSort_Diamond_RespectsAllDependencies()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();
        LayoutProperty c = Ref();
        LayoutProperty d = Ref();

        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([a, b, c, d], (a, b), (a, c), (b, d), (c, d)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c, d }));
        Assert.That(order.IndexOf(d), Is.LessThan(order.IndexOf(b)));
        Assert.That(order.IndexOf(d), Is.LessThan(order.IndexOf(c)));
        Assert.That(order.IndexOf(b), Is.LessThan(order.IndexOf(a)));
        Assert.That(order.IndexOf(c), Is.LessThan(order.IndexOf(a)));
    }

    [Test]
    public void TopologicalSort_NodeWithoutDependencies_IsIncluded()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();
        LayoutProperty c = Ref();

        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b)));

        Assert.That(order, Is.EquivalentTo(new[] { a, b, c }));
    }

    [Test]
    public void TopologicalSort_EveryNodeAppearsExactlyOnce()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();
        LayoutProperty c = Ref();

        List<LayoutProperty> order = RunTopologicalSort(BuildGraph([a, b, c], (a, b), (b, c)));

        Assert.That(order, Has.Exactly(1).EqualTo(a));
        Assert.That(order, Has.Exactly(1).EqualTo(b));
        Assert.That(order, Has.Exactly(1).EqualTo(c));
    }

    [Test]
    public void TopologicalSort_SelfLoop_Throws()
    {
        LayoutProperty a = Ref();

        Assert.That(
            () => RunTopologicalSort(BuildGraph([a], (a, a))),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    [Test]
    public void TopologicalSort_MutualCycle_Throws()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();

        Assert.That(
            () => RunTopologicalSort(BuildGraph([a, b], (a, b), (b, a))),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    [Test]
    public void TopologicalSort_ThreeNodeCycle_Throws()
    {
        LayoutProperty a = Ref();
        LayoutProperty b = Ref();
        LayoutProperty c = Ref();

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
        ((TestPos)b.X).References.Add(new LayoutProperty(LayoutPropertyType.X, a));

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
        ((TestPos)a.X).References.Add(new LayoutProperty(LayoutPropertyType.X, b));
        ((TestPos)b.X).References.Add(new LayoutProperty(LayoutPropertyType.X, a));

        Assert.That(
            () => LayoutSolver.Solve(parent),
            Throws.Exception.With.Message.EqualTo("Layout cycle detected!"));
    }

    private static LayoutProperty Ref()
    {
        return new LayoutProperty(LayoutPropertyType.X, new PanelElement { Title = "ref" });
    }

    private static (Queue<LayoutProperty> Queue, EdgeMap DependencyEdges, EdgeMap DependentEdges) BuildGraph(
        LayoutProperty[] nodes,
        params (LayoutProperty Source, LayoutProperty Target)[] edges)
    {
        Queue<LayoutProperty> queue = new();
        EdgeMap dependencyEdges = [];
        EdgeMap dependentEdges = [];
        HashSet<LayoutProperty> sources = [];

        foreach ((LayoutProperty source, LayoutProperty target) in edges)
        {
            AddEdge(dependencyEdges, source, target);
            AddEdge(dependentEdges, target, source);
            sources.Add(source);
        }

        foreach (LayoutProperty node in nodes)
        {
            if (!sources.Contains(node))
            {
                queue.Enqueue(node);
            }
        }

        return (queue, dependencyEdges, dependentEdges);
    }

    private static void AddEdge(EdgeMap edges, LayoutProperty source, LayoutProperty target)
    {
        if (!edges.TryGetValue(source, out HashSet<LayoutProperty>? targets))
        {
            targets = [];
            edges.Add(source, targets);
        }
        targets.Add(target);
    }

    private static List<LayoutProperty> RunTopologicalSort(
        (Queue<LayoutProperty> Queue, EdgeMap DependencyEdges, EdgeMap DependentEdges) graph)
    {
        return LayoutSolver.TopologicalSort(graph.Queue, graph.DependencyEdges, graph.DependentEdges);
    }

    private sealed class TestPos : Pos
    {
        public List<LayoutProperty> References { get; } = [];
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => References;
        internal override int Resolve(LayoutProperty property, List<int> dependencies) => 0;
    }

    private sealed class TestSize : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];
        internal override int Resolve(LayoutProperty property, List<int> dependencies) => 0;
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
