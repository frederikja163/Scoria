namespace Scoria.Layout;

using EdgeCollection = HashSet<Reference>;
using EdgeMap = Dictionary<Reference, HashSet<Reference>>;
using DependencyMap = Dictionary<Reference, List<Reference>>;

internal sealed class LayoutSolver
{
    private readonly List<Reference> _topologicalOrder;
    private readonly DependencyMap _dependencies;
    private readonly Dictionary<Reference, int> _solvedValues = [];

    internal LayoutSolver(List<Reference> topologicalOrder, DependencyMap dependencies)
    {
        _topologicalOrder = topologicalOrder;
        _dependencies = dependencies;
    }

    internal int GetValue(Reference reference)
    {
        if (_solvedValues.TryGetValue(reference, out int value))
        {
            return value;
        }

        throw new Exception("Layouts cannot reference properties from another viewport.");
    }

    internal void Solve()
    {
        foreach (Reference reference in _topologicalOrder)
        {
            IReferenceContainer property = reference.GetProperty();
            _solvedValues[reference] = property.Solve(this, _dependencies[reference].Select(GetValue).ToList());
        }
    }

    internal void PopulateProperties()
    {
        foreach ((Reference reference, int value) in _solvedValues)
        {
            reference.Element.CalculatedLayout.SetProperty(reference.Property, value);
        }
    }

    internal static void Solve(Element parentElement, bool includeSelf = false)
    {
        EdgeMap forwardEdges = [];
        EdgeMap backwardEdges = [];
        Queue<Reference> emptyQueue = [];
        Dictionary<Reference, List<Reference>> dependencies = [];
        foreach (Element element in GetChildrenRecursive(parentElement, includeSelf))
        {
            AddEdges(element.X.GetReferences(Property.X, element), new Reference(Property.X, element));
            AddEdges(element.Y.GetReferences(Property.Y, element), new Reference(Property.Y, element));
            AddEdges(element.Width.GetReferences(Property.Width, element), new Reference(Property.Width, element));
            AddEdges(element.Height.GetReferences(Property.Height, element), new Reference(Property.Height, element));
        }

        List<Reference> topologicalOrder = TopologicalSort(emptyQueue, forwardEdges, backwardEdges);
        LayoutSolver layoutSolver = new LayoutSolver(topologicalOrder, dependencies);
        layoutSolver.Solve();
        layoutSolver.PopulateProperties();
        
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

    internal static List<Reference> TopologicalSort(Queue<Reference> emptyQueue, EdgeMap forwardEdges, EdgeMap backwardEdges)
    {
        List<Reference> topologicalOrder = [];
        while (emptyQueue.TryDequeue(out Reference? node))
        {
            // Add to empty queue.
            topologicalOrder.Add(node);
            
            // Remove any edges going to this edge.
            if (!backwardEdges.TryGetValue(node, out EdgeCollection? sources))
                continue;
            foreach (Reference source in sources)
            {
                EdgeCollection targets = forwardEdges[source];
                targets.Remove(node);

                if (targets.Count == 0)
                {
                    forwardEdges.Remove(source);
                    emptyQueue.Enqueue(source);
                }
            }

            backwardEdges.Remove(node);
        }

        if (forwardEdges.Any())
        {
            throw new Exception("Layout cycle detected!");
        }

        return topologicalOrder;
    }

    private static void AddEdge(EdgeMap edges, Reference source, Reference target)
    {
        if (!edges.TryGetValue(source, out EdgeCollection? targets))
        {
            targets = [];
            edges.Add(source, targets);
        }
        targets.Add(target);
    }
}