using Scoria.Rendering;

namespace Scoria.Styling;

/// <summary>
/// Defines the visual appearance of surfaces and windows, including border, title, and background styles.
/// </summary>
public sealed class Theme : ThemeBase<Theme>
{
    /// <summary>Gets or sets the theme used by default for new surfaces and windows.</summary>
    public static Theme Default { get; set; } = new Theme();

    private Theme() { }
    private Theme(Theme original) : base(original) { }

    public override Theme Clone()
    {
        return new Theme(this);
    }

    private readonly BorderTheme _border = new BorderTheme();
    public BorderTheme Border { get => Get(_border.Clone, addIfNotFound: true); set => Set(value.Clone()); }

    /// <summary>Gets or sets the style used to fill the window background.</summary>
    public Style BackgroundStyle { get => Get(new Style(0, 0, 0, 20, 20, 20, 255)); set => Set(value); }
}

public sealed class BorderTheme : ThemeBase<BorderTheme>
{
    internal BorderTheme() { }
    private BorderTheme(BorderTheme original) : base(original) { }

    public IBorderResolver Resolver { get => Get(Borders.Instance); set => Set(value); }

    /// <summary>Gets or sets the style used to draw window and panel borders.</summary>
    public Style Style { get => Get(new Style(255, 255, 255, StyleAttributes.None)); set => Set(value); }
    /// <summary>Gets or sets the style used to draw border titles.</summary>
    public Style TitleStyle { get => Get(new Style(255, 255, 255, StyleAttributes.None)); set => Set(value); }

    public override BorderTheme Clone()
    {
        return new BorderTheme(this);
    }
}
