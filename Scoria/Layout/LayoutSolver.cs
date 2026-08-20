using Scoria.Elements;

namespace Scoria.Layout;

using EdgeCollection = HashSet<LayoutProperty>;
using EdgeMap = Dictionary<LayoutProperty, HashSet<LayoutProperty>>;
using DependencyMap = Dictionary<LayoutProperty, List<LayoutProperty>>;

internal sealed class LayoutSolver
{
    private readonly List<LayoutProperty> _topologicalOrder;
    private readonly DependencyMap _dependencies;
    private readonly Dictionary<LayoutProperty, int> _solvedValues = [];

    internal LayoutSolver(List<LayoutProperty> topologicalOrder, DependencyMap dependencies)
    {
        _topologicalOrder = topologicalOrder;
        _dependencies = dependencies;
    }

    internal int GetValue(LayoutProperty layoutProperty)
    {
        if (_solvedValues.TryGetValue(layoutProperty, out int value))
        {
            return value;
        }

        throw new LayoutException("Layouts cannot reference properties from another viewport.");
    }

    internal void Solve()
    {
        foreach (LayoutProperty property in _topologicalOrder)
        {
            ILayoutResolver resolver = property.GetProperty();
            _solvedValues[property] = resolver.Resolve(property, _dependencies[property].Select(GetValue).ToList());
        }
    }

    internal void PopulateProperties()
    {
        foreach ((LayoutProperty reference, int value) in _solvedValues)
        {
            reference.Element.CalculatedLayout.SetProperty(reference.Type, value);
        }
    }

    internal static void Solve(Element parentElement, bool includeSelf = false)
    {
        EdgeMap dependencyEdges = [];
        EdgeMap dependentEdges = [];
        Queue<LayoutProperty> emptyQueue = [];
        Dictionary<LayoutProperty, List<LayoutProperty>> dependencies = [];
        foreach (Element element in GetChildrenRecursive(parentElement, includeSelf))
        {
            AddEdges(new LayoutProperty(LayoutPropertyType.X, element));
            AddEdges(new LayoutProperty(LayoutPropertyType.Y, element));
            AddEdges(new LayoutProperty(LayoutPropertyType.Width, element));
            AddEdges(new LayoutProperty(LayoutPropertyType.Height, element));
        }

        List<LayoutProperty> topologicalOrder = TopologicalSort(emptyQueue, dependencyEdges, dependentEdges);
        LayoutSolver layoutSolver = new LayoutSolver(topologicalOrder, dependencies);
        layoutSolver.Solve();
        layoutSolver.PopulateProperties();
        
        void AddEdges(LayoutProperty source)
        {
            List<LayoutProperty> targets = source.GetProperty().GetDependencies(source);
            dependencies.Add(source, targets);
            if (!targets.Any())
            {
                emptyQueue.Enqueue(source);
            }
            foreach (LayoutProperty target in targets)
            {
                AddEdge(dependencyEdges, source, target);
                AddEdge(dependentEdges, target, source);
            }
        }
    }

    private static IEnumerable<Element> GetChildrenRecursive(Element element, bool includeParent)
    {
        if (includeParent)
        {
            yield return element;
        }
        foreach (Element child in element.GetChildren())
        {
            foreach (Element ele in GetChildrenRecursive(child, true))
            {
                yield return ele;
            }
        }
    }

    internal static List<LayoutProperty> TopologicalSort(Queue<LayoutProperty> emptyQueue, EdgeMap dependencyEdges, EdgeMap dependentEdges)
    {
        List<LayoutProperty> topologicalOrder = [];
        while (emptyQueue.TryDequeue(out LayoutProperty? node))
        {
            // Add to empty queue.
            topologicalOrder.Add(node);
            
            // Remove any edges going to this edge.
            if (!dependentEdges.TryGetValue(node, out EdgeCollection? sources))
                continue;
            foreach (LayoutProperty source in sources)
            {
                EdgeCollection targets = dependencyEdges[source];
                targets.Remove(node);

                if (targets.Count == 0)
                {
                    dependencyEdges.Remove(source);
                    emptyQueue.Enqueue(source);
                }
            }

            dependentEdges.Remove(node);
        }

        if (dependencyEdges.Any())
        {
            throw new LayoutCycleException(GetCycle(dependencyEdges));
        }

        return topologicalOrder;
    }

    private static IEnumerable<LayoutProperty> GetCycle(EdgeMap dependencyEdges)
    {
        LayoutProperty first = dependencyEdges.Keys.First();
        LayoutProperty? node = first;
        EdgeCollection? dependencies = dependencyEdges.GetValueOrDefault(node);
        do
        {
            yield return node;
            node = dependencies?.FirstOrDefault();
        } while (node is not null && node != first && dependencyEdges.TryGetValue(node, out dependencies));
        
        yield return first;

        if (dependencies is null)
        {
            throw new LayoutException($"{node} References a property outside the graph");
        }
    }

    private static void AddEdge(EdgeMap edges, LayoutProperty source, LayoutProperty target)
    {
        if (!edges.TryGetValue(source, out EdgeCollection? targets))
        {
            targets = [];
            edges.Add(source, targets);
        }
        targets.Add(target);
    }
}