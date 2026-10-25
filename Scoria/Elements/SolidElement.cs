using Scoria.Rendering;
using Scoria.Styling;

namespace Scoria.Elements;

public sealed class SolidElement : Element
{
    public Style Style { get; set; }
    public char Char { get; set; }

    protected override void Render(ISurface surface)
    {
        surface.SubSurface(CalculatedLayout).Fill(Char, Style);
        base.Render(surface);
    }
}
