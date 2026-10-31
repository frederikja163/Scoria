using Scoria;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;
using Scoria.Styling;

var window = new Window { Title = "Scoria Layout Demo" };

GridElement element = new GridElement(2, 2)
{
    RowGap = 1,
    ColumnGap = 2,
    FillMethod = GridFillMethod.ColumnThenRow,
};

element.AddChild(new InsetElement(10) { InnerColor = Color.White, OuterColor = Color.Blue, InnerAlpha = 100 }, 1..2, 0..1);
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
