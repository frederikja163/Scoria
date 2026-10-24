using Scoria;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;

Style blackOnWhite = new Style(0, 0, 0, 255, 255, 255);

var window = new Window { Title = "Scoria Layout Demo" };
//window.AddChild(new SolidElement() { Char = '#', Style = blackOnWhite });

GridElement element = new GridElement(3, 2)
{
    RowGap = 1,
    ColumnGap = 2,
    FillMethod = GridFillMethod.ColumnThenRow,
};

element.AddChild(new SolidElement() { Char = '1', Style = Style.Default }, 1, 0);
element.AddChild(new SolidElement() { Char = '2', Style = Style.Default });
element.AddChild(new SolidElement() { Char = '3', Style = Style.Default });
element.AddChild(new SolidElement() { Char = '4', Style = Style.Default });
element.AddChild(new SolidElement() { Char = '5', Style = Style.Default });
window.AddChild(element);


var app = new Application(new ApplicationOptions { Window = window });
window.Events.Add<KeyEventArgs>(e =>
{
    if (e.Key == Key.Q)
    {
        app.Stop();
    }
});
app.Start();
