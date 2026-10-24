using NUnit.Framework;
using Scoria;
using Scoria.Elements;
using Scoria.Layout;

namespace Scoria.Tests.Layout;

using EdgeMap = Dictionary<LayoutProperty, HashSet<LayoutProperty>>;

[TestFixture]
public class PosTests
{
    private static readonly Element DummyElement = NewElement();

    private static LayoutProperty Prop(LayoutPropertyType type) => new(type, DummyElement);

    [Test]
    public void Abs_GetDependencies_ReturnsEmpty()
    {
        Element self = NewElement();

        List<LayoutProperty> dependencies = Pos.Abs(10).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.Empty);
    }

    [Test]
    public void Abs_Resolve_ReturnsValue()
    {
        Assert.That(Pos.Abs(42).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(42));
    }

    [Test]
    public void Abs_Resolve_NegativeValue_ReturnsNegative()
    {
        Assert.That(Pos.Abs(-5).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(-5));
    }

    [Test]
    public void Abs_Resolve_IgnoresDependencies()
    {
        Assert.That(Pos.Abs(7).Resolve(Prop(LayoutPropertyType.X), [999, -5, 3]), Is.EqualTo(7));
    }

    [Test]
    public void Auto_GetDependencies_UsesElementDefault()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        List<LayoutProperty> dependencies = Pos.Auto().GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, parent),
            new LayoutProperty(LayoutPropertyType.Width, parent),
        }));
    }

    [Test]
    public void Auto_Resolve_UsesElementDefault()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        // Element's default for X is Pos.Center(): centers a 20-wide element within a 100-wide reference at position 0.
        Assert.That(Pos.Auto().Resolve(new LayoutProperty(LayoutPropertyType.X, self), [20, 0, 100]), Is.EqualTo(40));
    }

    [Test]
    public void Relative_GetDependencies_XAxis_ReferencesSelfSizeThenReferencePositionThenReferenceSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.Relative(0.5f, reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void Relative_GetDependencies_YAxis_ReferencesSelfHeightThenReferenceYThenReferenceHeight()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.Relative(0.5f, reference).GetDependencies(new LayoutProperty(LayoutPropertyType.Y, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Height, self),
            new LayoutProperty(LayoutPropertyType.Y, reference),
            new LayoutProperty(LayoutPropertyType.Height, reference),
        }));
    }

    [Test]
    public void Relative_GetDependencies_UsesParentWhenNoElementSpecified()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        List<LayoutProperty> dependencies = Pos.Relative(0.5f).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, parent),
            new LayoutProperty(LayoutPropertyType.Width, parent),
        }));
    }

    [Test]
    public void Relative_GetDependencies_WithoutParentOrElement_Throws()
    {
        Element self = NewElement();

        Assert.That(
            () => Pos.Relative(0.5f).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self)),
            Throws.TypeOf<LayoutException>());
    }

    [Test]
    public void Relative_Resolve_ZeroPercent_AlignsToReferencePosition()
    {
        Assert.That(Pos.Relative(0f).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(50));
    }

    [Test]
    public void Relative_Resolve_HalfPercent_OffsetsByHalfOfRemainingSpace()
    {
        Assert.That(Pos.Relative(0.5f).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(55));
    }

    [Test]
    public void Relative_Resolve_HundredPercent_AlignsToReferenceEnd()
    {
        Assert.That(Pos.Relative(1f).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(60));
    }

    [Test]
    public void Relative_Resolve_TruncatesFractionalResultTowardsZero()
    {
        Assert.That(Pos.Relative(0.49f).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(54));
    }

    [Test]
    public void Relative_Resolve_NegativeRemainingSpace_Works()
    {
        Assert.That(Pos.Relative(0.5f).Resolve(Prop(LayoutPropertyType.X), [20, 50, 10]), Is.EqualTo(45));
    }

    [Test]
    public void Center_GetDependencies_WithReferenceElement_UsesSpecifiedElement()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.Center(reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void Center_Resolve_CentersWithinReference()
    {
        Assert.That(Pos.Center().Resolve(Prop(LayoutPropertyType.X), [20, 0, 100]), Is.EqualTo(40));
    }

    [Test]
    public void Center_Resolve_OffsetsByReferencePosition()
    {
        Assert.That(Pos.Center().Resolve(Prop(LayoutPropertyType.X), [20, 10, 100]), Is.EqualTo(50));
    }

    [Test]
    public void Center_Resolve_RoundsDownForOddRemainder()
    {
        Assert.That(Pos.Center().Resolve(Prop(LayoutPropertyType.X), [20, 0, 101]), Is.EqualTo(40));
    }

    [Test]
    public void Begin_GetDependencies_ReferencesReferencePosition()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.Begin(element: reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.X, reference),
        }));
    }

    [Test]
    public void Begin_Resolve_AlignsToReferencePosition()
    {
        Assert.That(Pos.Begin().Resolve(Prop(LayoutPropertyType.X), [50]), Is.EqualTo(50));
    }

    [Test]
    public void End_GetDependencies_ReferencesSelfSizeThenReferencePositionThenReferenceSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.End(element: reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void End_Resolve_AlignsToReferenceEnd()
    {
        Assert.That(Pos.End().Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(60));
    }

    [Test]
    public void Before_GetDependencies_ReferencesReferencePositionThenSelfSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.Before(element: reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, self),
        }));
    }

    [Test]
    public void Before_Resolve_PlacesElementBeforeReference()
    {
        Assert.That(Pos.Before().Resolve(Prop(LayoutPropertyType.X), [50, 20]), Is.EqualTo(30));
    }

    [Test]
    public void After_GetDependencies_ReferencesReferencePositionThenReferenceSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Pos.After(element: reference).GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void After_Resolve_PlacesElementAfterReference()
    {
        Assert.That(Pos.After().Resolve(Prop(LayoutPropertyType.X), [50, 30]), Is.EqualTo(80));
    }

    [Test]
    public void Solve_AbsoluteChild_ResolvesToGivenValues()
    {
        Element parent = NewElement();
        Element child = NewElement();
        parent.AddChild(child);
        child.X = Pos.Abs(11);
        child.Y = Pos.Abs(22);
        child.Width = Size.Abs(33);
        child.Height = Size.Abs(44);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(child.CalculatedLayout.X, Is.EqualTo(11));
        Assert.That(child.CalculatedLayout.Y, Is.EqualTo(22));
        Assert.That(child.CalculatedLayout.Width, Is.EqualTo(33));
        Assert.That(child.CalculatedLayout.Height, Is.EqualTo(44));
    }

    [Test]
    public void Solve_CenteredChild_ResolvesToCenterOfParent()
    {
        Element parent = NewElement(width: 100, height: 80);
        Element child = NewElement();
        parent.AddChild(child);
        child.X = Pos.Center();
        child.Y = Pos.Center();
        child.Width = Size.Abs(20);
        child.Height = Size.Abs(10);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(child.CalculatedLayout.X, Is.EqualTo(40));
        Assert.That(child.CalculatedLayout.Y, Is.EqualTo(35));
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
        sibling.Width = Size.Abs(30);
        sibling.Height = Size.Abs(20);

        child.X = Pos.Relative(0.5f, sibling);
        child.Y = Pos.Relative(0.5f, sibling);
        child.Width = Size.Abs(4);
        child.Height = Size.Abs(4);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(child.CalculatedLayout.X, Is.EqualTo(63));
        Assert.That(child.CalculatedLayout.Y, Is.EqualTo(18));
    }

    [Test]
    public void Solve_ChildBeforeAndAfterSibling_ResolvesAroundSibling()
    {
        Element parent = NewElement();
        Element sibling = NewElement();
        Element child = NewElement();
        parent.AddChild(sibling);
        parent.AddChild(child);

        sibling.X = Pos.Abs(50);
        sibling.Y = Pos.Abs(10);
        sibling.Width = Size.Abs(30);
        sibling.Height = Size.Abs(20);

        child.X = Pos.Before(element: sibling);
        child.Y = Pos.After(element: sibling);
        child.Width = Size.Abs(4);
        child.Height = Size.Abs(4);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(child.CalculatedLayout.X, Is.EqualTo(46));
        Assert.That(child.CalculatedLayout.Y, Is.EqualTo(30));
    }

    [Test]
    public void Solve_ChildAlignedToSibling_ResolvesToSiblingBeginAndEnd()
    {
        Element parent = NewElement();
        Element sibling = NewElement();
        Element beginChild = NewElement();
        Element endChild = NewElement();
        parent.AddChild(sibling);
        parent.AddChild(beginChild);
        parent.AddChild(endChild);

        sibling.X = Pos.Abs(50);
        sibling.Y = Pos.Abs(10);
        sibling.Width = Size.Abs(30);
        sibling.Height = Size.Abs(20);

        beginChild.X = Pos.Begin(element: sibling);
        beginChild.Y = Pos.End(element: sibling);
        beginChild.Width = Size.Abs(4);
        beginChild.Height = Size.Abs(4);

        endChild.X = Pos.End(element: sibling);
        endChild.Y = Pos.Begin(element: sibling);
        endChild.Width = Size.Abs(4);
        endChild.Height = Size.Abs(4);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(beginChild.CalculatedLayout.X, Is.EqualTo(50));
        Assert.That(beginChild.CalculatedLayout.Y, Is.EqualTo(26));
        Assert.That(endChild.CalculatedLayout.X, Is.EqualTo(76));
        Assert.That(endChild.CalculatedLayout.Y, Is.EqualTo(10));
    }

    [Test]
    public void Operator_Add_CombinesResolvedValues()
    {
        Assert.That((Pos.Abs(5) + Pos.Abs(3)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(8));
    }

    [Test]
    public void Operator_Subtract_CombinesResolvedValues()
    {
        Assert.That((Pos.Abs(10) - Pos.Abs(4)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(6));
    }

    [Test]
    public void Operator_Multiply_CombinesResolvedValues()
    {
        Assert.That((Pos.Abs(4) * Pos.Abs(3)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(12));
    }

    [Test]
    public void Operator_Divide_CombinesResolvedValues()
    {
        Assert.That((Pos.Abs(7) / Pos.Abs(2)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(3));
    }

    [Test]
    public void Min_Resolve_ReturnsSmallerValue()
    {
        Assert.That(Pos.Min(Pos.Abs(5), Pos.Abs(3)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(3));
        Assert.That(Pos.Min(Pos.Abs(3), Pos.Abs(5)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(3));
    }

    [Test]
    public void Max_Resolve_ReturnsLargerValue()
    {
        Assert.That(Pos.Max(Pos.Abs(5), Pos.Abs(3)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(5));
        Assert.That(Pos.Max(Pos.Abs(3), Pos.Abs(5)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(5));
    }

    [Test]
    public void Min_Max_WithRelative_CompareResolvedValues()
    {
        Element reference = NewElement();

        // Abs(60) vs Center of [20, 50, 30] = 55.
        Assert.That(Pos.Min(Pos.Abs(60), Pos.Center(reference)).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(55));
        Assert.That(Pos.Max(Pos.Abs(60), Pos.Center(reference)).Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(60));
    }

    [Test]
    public void Min_WithImplicitInt_ComparesResolvedValue()
    {
        Assert.That(Pos.Min(Pos.Abs(100), 50).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(50));
    }

    [Test]
    public void Min_GetDependencies_ConcatenatesOperandDependencies()
    {
        Element self = NewElement();
        Element reference = NewElement();

        Pos pos = Pos.Min(Pos.Abs(5), Pos.Center(reference));
        List<LayoutProperty> dependencies = pos.GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void Operator_Add_SizeToPos_DisplacesPosition()
    {
        Assert.That((Pos.Abs(3) + Size.Abs(4)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(7));
        Assert.That((Size.Abs(3) + Pos.Abs(4)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(7));
    }

    [Test]
    public void Operator_Add_SizeToRelativePos_CombinesResolvedValues()
    {
        Element reference = NewElement();

        // Begin resolves to the reference's start (50); Size.Relative(0.5) adds half of the reference width (15).
        Pos pos = Pos.Begin(element: reference) + Size.Relative(0.5f, reference);

        Assert.That(pos.Resolve(Prop(LayoutPropertyType.X), [50, 30]), Is.EqualTo(65));
    }

    [Test]
    public void Operator_Subtract_SizeFromPos_DisplacesPosition()
    {
        Assert.That((Pos.Abs(10) - Size.Abs(3)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(7));
    }

    [Test]
    public void Operator_Int_Literals_RemainUnambiguous()
    {
        Assert.That((Pos.Abs(5) + 3).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(8));
        Assert.That((3 + Pos.Abs(5)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(8));
    }

    [Test]
    public void Operator_GetDependencies_ConcatenatesOperandDependencies()
    {
        Element self = NewElement();
        Element reference = NewElement();

        Pos pos = Pos.Relative(0.5f, reference) + Pos.Relative(0.25f, reference);
        List<LayoutProperty> dependencies = pos.GetDependencies(new LayoutProperty(LayoutPropertyType.X, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
            new LayoutProperty(LayoutPropertyType.Width, self),
            new LayoutProperty(LayoutPropertyType.X, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void Operator_Resolve_SplitsDependenciesPerOperand()
    {
        Element reference = NewElement();
        Pos pos = Pos.Relative(0.5f, reference) + Pos.Relative(0.25f, reference);

        Assert.That(pos.Resolve(Prop(LayoutPropertyType.X), [20, 50, 30, 20, 50, 30]), Is.EqualTo(107));
    }

    [Test]
    public void Operator_Add_AbsoluteAndRelative_CombinesResolvedValues()
    {
        Element self = NewElement();
        Element reference = NewElement();
        Pos pos = (Pos.Begin(element: reference) + Pos.Abs(3)) * 2;

        Assert.That(pos.Resolve(new LayoutProperty(LayoutPropertyType.X, self), [20, 50, 30]), Is.EqualTo(46));
    }

    [Test]
    public void Implicit_Int_ConvertsToAbsolutePosition()
    {
        Pos pos = 5;

        Assert.That(pos.Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(5));
    }

    [Test]
    public void Implicit_Int_InExpressions_ResolvesAsAbsolute()
    {
        Assert.That((Pos.Abs(5) + 3).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(8));
        Assert.That((2 * Pos.Abs(5)).Resolve(Prop(LayoutPropertyType.X), []), Is.EqualTo(10));
    }

    [Test]
    public void Implicit_Float_ConvertsToRelativePosition()
    {
        Pos pos = 0.5f;

        Assert.That(pos.Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(55));
    }

    [Test]
    public void Implicit_Double_ConvertsToRelativePosition()
    {
        Pos pos = 0.5;

        Assert.That(pos.Resolve(Prop(LayoutPropertyType.X), [20, 50, 30]), Is.EqualTo(55));
    }

    [Test]
    public void Solve_CombinedPositions_ResolveToCombinedValues()
    {
        Element parent = NewElement(width: 100, height: 80);
        Element child = NewElement();
        parent.AddChild(child);
        child.X = Pos.Abs(10) + Pos.Abs(5);
        child.Y = Pos.Center() + 2;
        child.Width = Size.Abs(2) * Size.Abs(3);
        child.Height = Size.Abs(2) + Size.Abs(4);

        LayoutSolver solver = BuildSolver(parent);
        solver.Solve();

        Assert.That(child.CalculatedLayout.X, Is.EqualTo(15));
        Assert.That(child.CalculatedLayout.Y, Is.EqualTo(39));
        Assert.That(child.CalculatedLayout.Width, Is.EqualTo(6));
        Assert.That(child.CalculatedLayout.Height, Is.EqualTo(6));
    }

    private static LayoutSolver BuildSolver(Element root)
    {
        EdgeMap dependencyEdges = [];
        EdgeMap dependentEdges = [];
        Queue<LayoutProperty> emptyQueue = [];
        Dictionary<LayoutProperty, List<LayoutProperty>> dependencies = [];
        foreach (Element element in GetElementsRecursive(root))
        {
            AddEdges(element.X.GetDependencies(new LayoutProperty(LayoutPropertyType.X, element)), new LayoutProperty(LayoutPropertyType.X, element));
            AddEdges(element.Y.GetDependencies(new LayoutProperty(LayoutPropertyType.Y, element)), new LayoutProperty(LayoutPropertyType.Y, element));
            AddEdges(element.Width.GetDependencies(new LayoutProperty(LayoutPropertyType.Width, element)), new LayoutProperty(LayoutPropertyType.Width, element));
            AddEdges(element.Height.GetDependencies(new LayoutProperty(LayoutPropertyType.Height, element)), new LayoutProperty(LayoutPropertyType.Height, element));
        }

        List<LayoutProperty> topologicalOrder = LayoutSolver.TopologicalSort(emptyQueue, dependencyEdges, dependentEdges);
        return new LayoutSolver(topologicalOrder, dependencies);

        void AddEdges(List<LayoutProperty> targets, LayoutProperty source)
        {
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

    private static void AddEdge(EdgeMap edges, LayoutProperty source, LayoutProperty target)
    {
        if (!edges.TryGetValue(source, out HashSet<LayoutProperty>? targets))
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

    private static Element NewElement(int width = 0, int height = 0)
    {
        return new PanelElement
        {
            Title = "element",
            X = Pos.Abs(0),
            Y = Pos.Abs(0),
            Width = Size.Abs(width),
            Height = Size.Abs(height),
        };
    }
}
