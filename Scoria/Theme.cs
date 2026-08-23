namespace Scoria;

/// <summary>
/// Defines the visual appearance of surfaces and windows, including border, title, and background styles.
/// </summary>
public sealed class Theme
{
    /// <summary>Gets or sets the theme used by default for new surfaces and windows.</summary>
    public static Theme Default { get; set; } = new Theme();

    /// <summary>Gets or sets the border renderer used to draw and expand borders.</summary>
    public IBorders Borders { get; set; } = new Borders();

    /// <summary>Gets or sets the style used to draw window and panel borders.</summary>
    public Style Border { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
    /// <summary>Gets or sets the style used to draw border titles.</summary>
    public Style Title { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
    /// <summary>Gets or sets the style used to fill the window background.</summary>
    public Style Background { get; set; } = new Style(0, 0, 0, 20, 20, 20, 255);
}