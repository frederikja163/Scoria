namespace Scoria;

public sealed class Theme
{
    public static Theme CurrentTheme { get; set; } = new Theme();

    public Borders Borders { get; set; } = new Borders();

    public Style BorderStyle { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
    public Style TitleStyle { get; set; } = new Style(255, 255, 255, StyleAttributes.None);
}