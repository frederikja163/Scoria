using Scoria;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;
using Scoria.Styling;

var window = new Window { Title = "Scoria Layout Demo" };
//window.AddChild(new SolidElement() { Char = '#', Style = blackOnWhite });

GridElement element = new GridElement(2, 2)
{
    RowGap = 1,
    ColumnGap = 2,
    FillMethod = GridFillMethod.ColumnThenRow,
};

element.AddChild(new InsetElement(1) { InnerColor = Color.White, OuterColor = Color.White }, 1..2, 0..1);
//window.AddChild(element);


var app = new Application(new ApplicationOptions { Window = window });
window.Events.Add<KeyEventArgs>(e =>
{
    if (e.Key == Key.Q)
    {
        app.Stop();
    }
});
app.Start();
