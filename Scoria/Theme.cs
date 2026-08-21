namespace Scoria;

public sealed class Theme
{
    public static Theme Default { get; set; } = new Theme();

    public IBorders Borders { get; set; } = new Borders();

    public Style Border { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
    public Style Title { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
    public Style Background { get; set; } = new Style(0, 0, 0, 20, 20, 20, 255);
}