using NUnit.Framework;
using Scoria;
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
    public void Auto_GetDependencies_ReturnsElementAutoLayoutDependencies()
    {
        Element self = NewElement();

        List<LayoutProperty> dependencies = Size.Auto().GetDependencies(new LayoutProperty(LayoutPropertyType.Width, self));

        Assert.That(dependencies, Is.Empty);
    }

    [Test]
    public void Auto_Resolve_ReturnsElementAutoLayoutValue()
    {
        Assert.That(Size.Auto().Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(0));
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

        Assert.That(Size.Auto().Resolve(new LayoutProperty(LayoutPropertyType.Width, element), []), Is.EqualTo(5));
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
            Throws.Exception.With.Message.EqualTo("Relative size must either specify an element or have a parent element."));
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
    public void FitChildren_GetDependencies_ReferencesEachChild()
    {
        Element parent = NewElement();
        Element a = NewElement();
        Element b = NewElement();
        parent.AddChild(a);
        parent.AddChild(b);

        List<LayoutProperty> dependencies = Size.FitChildren().GetDependencies(new LayoutProperty(LayoutPropertyType.Width, parent));

        Assert.That(dependencies, Is.EqualTo(new[]
        {
            new LayoutProperty(LayoutPropertyType.Width, a),
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
    public void FitChildren_Resolve_SumsChildSizes()
    {
        Assert.That(Size.FitChildren().Resolve(Prop(LayoutPropertyType.Width), [10, 20]), Is.EqualTo(30));
    }

    [Test]
    public void FitChildren_Resolve_NoChildren_ReturnsZero()
    {
        Assert.That(Size.FitChildren().Resolve(Prop(LayoutPropertyType.Width), []), Is.EqualTo(0));
    }

    private static Element NewElement()
    {
        return new PanelElement { Title = "element" };
    }
}
