using NUnit.Framework;
using Scoria;
using Scoria.Elements;
using Scoria.Layout;

namespace Scoria.Tests.Layout;

[TestFixture]
public class SizeTests
{
    private static readonly Element DummyElement = NewElement();

    private static LayoutProperty Prop(LayoutPropertyType type) => new(type, DummyElement);

    [Test]
    public void Abs_GetDependencies_ReturnsEmpty()
    {
        Element self = NewElement();

        List<LayoutProperty> dependencies = Size.Abs(10).GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.Empty);
    }

    [Test]
    public void Abs_Resolve_ReturnsValue()
    {
        Assert.That(Size.Abs(42).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(42));
    }

    [Test]
    public void Abs_Resolve_NegativeValue_ReturnsNegative()
    {
        Assert.That(Size.Abs(-5).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(-5));
    }

    [Test]
    public void Abs_Resolve_IgnoresDependencies()
    {
        Assert.That(Size.Abs(7).Resolve(Prop(LayoutPropertyType.Width), [999, -5, 3]), Is.EqualTo(7));
    }

    [Test]
    public void Auto_GetDependencies_UsesElementDefault()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        List<LayoutProperty> dependencies = Size.Auto().GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.EqualTo(new[] { new LayoutProperty(LayoutPropertyType.Width, parent) }));
    }

    [Test]
    public void Auto_Resolve_UsesElementDefault()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        // Element's default for Width is Size.Fill(): matches the reference size.
        Assert.That(Size.Auto().Resolve(new LayoutProperty(LayoutPropertyType.Width, self), [100]), Is.EqualTo(100));
    }

    [Test]
    public void Auto_GetDependencies_TextElementHeight_ReferencesTextElementWidth()
    {
        TextElement self = new() { Text = "Hello" };

        List<LayoutProperty> dependencies = Size.Auto().GetDependencies(new LayoutProperty(LayoutPropertyType.Height, self));

        Assert.That(dependencies, Is.EqualTo(new[] { new LayoutProperty(LayoutPropertyType.Width, self) }));
    }

    [Test]
    public void Auto_Resolve_TextElementWidth_UsesTextLength()
    {
        TextElement element = new() { Text = "Hello" };

        Assert.That(Size.Auto().Resolve(new LayoutProperty(LayoutPropertyType.Width, element), [10]), Is.EqualTo(5));
    }

    [Test]
    public void Auto_Resolve_TextElementWidth_ClampedToParentWidth()
    {
        TextElement element = new() { Text = "Hello" };

        Assert.That(Size.Auto().Resolve(new LayoutProperty(LayoutPropertyType.Width, element), [3]), Is.EqualTo(3));
    }

    [Test]
    public void Auto_Resolve_TextElementHeight_RoundsUpToWholeLines()
    {
        TextElement element = new() { Text = "Hello world" };

        Assert.That(Size.Auto().Resolve(new LayoutProperty(LayoutPropertyType.Height, element), [4]), Is.EqualTo(3));
    }

    [Test]
    public void Relative_GetDependencies_ReferencesReferenceSize()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = Size.Relative(0.5f, reference).GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.EqualTo(new[] { new LayoutProperty(LayoutPropertyType.Width, reference) }));
    }

    [Test]
    public void Relative_GetDependencies_UsesParentWhenNoElementSpecified()
    {
        Element parent = NewElement();
        Element self = NewElement();
        parent.AddChild(self);

        List<LayoutProperty> dependencies = Size.Relative(0.5f).GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.EqualTo(new[] { new LayoutProperty(LayoutPropertyType.Width, parent) }));
    }

    [Test]
    public void Relative_GetDependencies_WithoutParentOrElement_Throws()
    {
        Element self = NewElement();

        Assert.That(
            () => Size.Relative(0.5f).GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self)),
            Throws.TypeOf<LayoutException>());
    }

    [Test]
    public void Relative_Resolve_MultipliesReferenceSize()
    {
        Assert.That(Size.Relative(0.5f).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(50));
    }

    [Test]
    public void Relative_Resolve_ZeroPercent_ReturnsZero()
    {
        Assert.That(Size.Relative(0f).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(0));
    }

    [Test]
    public void Relative_Resolve_HundredPercent_ReturnsFullReferenceSize()
    {
        Assert.That(Size.Relative(1f).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(100));
    }

    [Test]
    public void Relative_Resolve_TruncatesFractionalResultTowardsZero()
    {
        Assert.That(Size.Relative(0.33f).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(33));
    }

    [Test]
    public void Fill_Resolve_ReturnsFullReferenceSize()
    {
        Assert.That(Size.Fill().Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(100));
    }

    [Test]
    public void Aspect_GetDependencies_ReferencesOtherAxis()
    {
        Element self = NewElement();

        List<LayoutProperty> dependencies = Size.Aspect(2f).GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.EqualTo(new[] { new LayoutProperty(LayoutPropertyType.Height, self) }));
    }

    [Test]
    public void Aspect_Resolve_MultipliesByAspectRatio()
    {
        Assert.That(Size.Aspect(2f).Resolve(Prop(LayoutPropertyType.Width), [50]), Is.EqualTo(100));
    }

    [Test]
    public void Aspect_Resolve_HalfRatio_ReturnsHalfOfOtherAxis()
    {
        Assert.That(Size.Aspect(0.5f).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(50));
    }

    [Test]
    public void Aspect_Resolve_TruncatesFractionalResultTowardsZero()
    {
        Assert.That(Size.Aspect(0.5f).Resolve(Prop(LayoutPropertyType.Width), [99]), Is.EqualTo(49));
    }

    [Test]
    public void FitChildren_GetDependencies_ReferencesEachChildPositionAndSize()
    {
        Element parent = NewElement();
        Element a = NewElement();
        Element b = NewElement();
        parent.AddChild(a);
        parent.AddChild(b);

        List<LayoutProperty> dependencies = Size.FitChildren().GetDependencies(new LayoutProperty(LayoutPropertyType.Width, parent));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.X, a),
            new LayoutProperty(LayoutPropertyType.Width, a),
            new LayoutProperty(LayoutPropertyType.X, b),
            new LayoutProperty(LayoutPropertyType.Width, b),
        }));
    }

    [Test]
    public void FitChildren_GetDependencies_NoChildren_ReturnsEmpty()
    {
        Element parent = NewElement();

        List<LayoutProperty> dependencies = Size.FitChildren().GetDependencies(new LayoutProperty(LayoutPropertyType.Width, parent));

        Assert.That(dependencies, Is.Empty);
    }

    [Test]
    public void FitChildren_GetDependencies_YAxis_ReferencesChildYAndHeight()
    {
        Element parent = NewElement();
        Element a = NewElement();
        parent.AddChild(a);

        List<LayoutProperty> dependencies = Size.FitChildren().GetDependencies(new LayoutProperty(LayoutPropertyType.Height, parent));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Y, a),
            new LayoutProperty(LayoutPropertyType.Height, a),
        }));
    }

    [Test]
    public void FitChildren_Resolve_ComputesBoundingBox()
    {
        Element parent = NewElement();
        parent.AddChild(NewElement());
        parent.AddChild(NewElement());

        // child0: pos=10, size=5 → spans [10, 15]
        // child1: pos=3, size=8 → spans [3, 11]
        // bounding box: min=3, max=15 → 15-3 = 12
        Assert.That(Size.FitChildren().Resolve(new LayoutProperty(LayoutPropertyType.Width, parent), [10, 5, 3, 8]), Is.EqualTo(12));
    }

    [Test]
    public void FitChildren_Resolve_StackedChildren_ReturnsSameAsSum()
    {
        Element parent = NewElement();
        parent.AddChild(NewElement());
        parent.AddChild(NewElement());

        // child0: pos=0, size=10 → spans [0, 10]
        // child1: pos=10, size=20 → spans [10, 30]
        // bounding box: min=0, max=30 → 30
        Assert.That(Size.FitChildren().Resolve(new LayoutProperty(LayoutPropertyType.Width, parent), [0, 10, 10, 20]), Is.EqualTo(30));
    }

    [Test]
    public void FitChildren_Resolve_OverlappingChildren_ReturnsUnionExtent()
    {
        Element parent = NewElement();
        parent.AddChild(NewElement());
        parent.AddChild(NewElement());

        // child0: pos=0, size=15 → spans [0, 15]
        // child1: pos=5, size=15 → spans [5, 20]
        // bounding box: min=0, max=20 → 20
        Assert.That(Size.FitChildren().Resolve(new LayoutProperty(LayoutPropertyType.Width, parent), [0, 15, 5, 15]), Is.EqualTo(20));
    }

    [Test]
    public void FitChildren_Resolve_SingleChild_ReturnsChildSize()
    {
        Element parent = NewElement();
        parent.AddChild(NewElement());

        Assert.That(Size.FitChildren().Resolve(new LayoutProperty(LayoutPropertyType.Width, parent), [7, 12]), Is.EqualTo(12));
    }

    [Test]
    public void FitChildren_Resolve_NoChildren_ReturnsZero()
    {
        Assert.That(Size.FitChildren().Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(0));
    }

    [Test]
    public void ToString_Auto_ReturnsAuto()
    {
        Assert.That(Size.Auto().ToString(), Is.EqualTo("Auto"));
    }

    [Test]
    public void ToString_Abs_ReturnsAbsoluteValue()
    {
        Assert.That(Size.Abs(10).ToString(), Is.EqualTo("Absolute(10)"));
    }

    [Test]
    public void ToString_Relative_UsesParentWhenNoElement()
    {
        Assert.That(Size.Relative(0.5f).ToString(), Is.EqualTo("Relative(0.5, Parent)"));
    }

    [Test]
    public void ToString_Relative_UsesElementName()
    {
        Assert.That(Size.Relative(0.5f, new NamedElement("target")).ToString(), Is.EqualTo("Relative(0.5, target)"));
    }

    [Test]
    public void ToString_Fill_UsesParentWhenNoElement()
    {
        Assert.That(Size.Fill().ToString(), Is.EqualTo("Fill(Parent)"));
    }

    [Test]
    public void ToString_Aspect_ReturnsRatio()
    {
        Assert.That(Size.Aspect(2f).ToString(), Is.EqualTo("Aspect(2)"));
    }

    [Test]
    public void ToString_FitChildren_ReturnsFitChildren()
    {
        Assert.That(Size.FitChildren().ToString(), Is.EqualTo("FitChildren"));
    }

    [Test]
    public void Operator_Add_CombinesResolvedValues()
    {
        Assert.That((Size.Abs(5) + Size.Abs(3)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(8));
    }

    [Test]
    public void Operator_Subtract_CombinesResolvedValues()
    {
        Assert.That((Size.Abs(10) - Size.Abs(4)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(6));
    }

    [Test]
    public void Operator_Multiply_CombinesResolvedValues()
    {
        Assert.That((Size.Abs(4) * Size.Abs(3)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(12));
    }

    [Test]
    public void Operator_Divide_CombinesResolvedValues()
    {
        Assert.That((Size.Abs(7) / Size.Abs(2)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(3));
    }

    [Test]
    public void Operator_GetDependencies_ConcatenatesOperandDependencies()
    {
        Element self = NewElement();
        Element reference = NewElement();

        List<LayoutProperty> dependencies = (Size.Relative(0.5f, reference) + Size.Relative(0.25f, reference))
            .GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, reference),
            new LayoutProperty(LayoutPropertyType.Width, reference),
        }));
    }

    [Test]
    public void Operator_Resolve_SplitsDependenciesPerOperand()
    {
        Element reference = NewElement();
        Size size = Size.Relative(0.5f, reference) + Size.Relative(0.25f, reference);

        Assert.That(size.Resolve(Prop(LayoutPropertyType.Width), [100, 200]), Is.EqualTo(100));
    }

    [Test]
    public void Operator_Add_AbsoluteAndRelative_CombinesResolvedValues()
    {
        Element reference = NewElement();

        Assert.That((Size.Abs(5) + Size.Relative(0.5f, reference)).Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(55));
    }

    [Test]
    public void Operator_Expression_FitChildrenTimesDoublePlusInt_Resolves()
    {
        Element grandparent = NewElement();
        Element self = NewElement();
        grandparent.AddChild(self);
        self.AddChild(NewElement());
        self.AddChild(NewElement());

        Size size = Size.FitChildren() * 2.0 + 5;

        // FitChildren() resolves to 12 from [10, 5, 3, 8]; 2.0 resolves to 200 from the parent width 100.
        Assert.That(size.Resolve(new LayoutProperty(LayoutPropertyType.Width, self), [10, 5, 3, 8, 100]), Is.EqualTo(2405));
        Assert.That(size.ToString(), Is.EqualTo("((FitChildren * Relative(2, Parent)) + Absolute(5))"));
    }

    [Test]
    public void Operator_Int_Literals_RemainUnambiguous()
    {
        Assert.That((Size.Abs(5) + 3).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(8));
        Assert.That((3 + Size.Abs(5)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(8));
    }

    [Test]
    public void Implicit_Int_ConvertsToAbsoluteSize()
    {
        Size size = 5;

        Assert.That(size.Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(5));
    }

    [Test]
    public void Implicit_Int_InExpressions_ResolvesAsAbsolute()
    {
        Assert.That((Size.Abs(5) + 3).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(8));
        Assert.That((2 * Size.Abs(5)).Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(10));
    }

    [Test]
    public void Implicit_Float_ConvertsToRelativeSize()
    {
        Size size = 0.5f;

        Assert.That(size.Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(50));
    }

    [Test]
    public void Implicit_Double_ConvertsToRelativeSize()
    {
        Size size = 0.5;

        Assert.That(size.Resolve(Prop(LayoutPropertyType.Width), [100]), Is.EqualTo(50));
    }

    [Test]
    public void ToString_Combined_ReturnsParenthesizedExpression()
    {
        Assert.That((Size.Abs(5) + Size.Abs(3)).ToString(), Is.EqualTo("(Absolute(5) + Absolute(3))"));
    }

    [Test]
    public void ToString_Combined_WithImplicitInt()
    {
        Assert.That((Size.Abs(10) - 3).ToString(), Is.EqualTo("(Absolute(10) - Absolute(3))"));
    }

    private static Element NewElement()
    {
        return new PanelElement { Title = "element" };
    }

    private sealed class NamedElement(string name) : Element
    {
        public override string ToString() => name;
    }
}
