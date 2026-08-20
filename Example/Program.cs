using System.Text;
using Scoria;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var window = new Window { Title = "Scoria Layout Demo" };

Style titleStyle    = new Style(255, 255, 100, 20, 20, 30, 255, StyleAttributes.Bold);
Style subtitleStyle = new Style(160, 220, 255, 20, 20, 30);
Style labelStyle    = new Style(100, 255, 160, 20, 20, 30, 255, StyleAttributes.Bold);
Style valueStyle    = new Style(255, 255, 255, 20, 20, 30);
Style accentStyle   = new Style(255, 180, 100, 20, 20, 30);
Style dimStyle      = new Style(140, 140, 160, 20, 20, 30);
Style statusStyle   = new Style(100, 255, 100, 20, 20, 30, 255, StyleAttributes.Bold);
Style sidebarItemStyle = new Style(200, 200, 255, 20, 20, 40, 255, StyleAttributes.Bold);

int headerHeight = 5;
int sidebarWidth = 24;

// ═══════════════════════════════════════════════════════════════════════
// HEADER – fixed 5 rows tall, fills parent width
// ═══════════════════════════════════════════════════════════════════════
PanelElement header = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.Abs(0),
    Width = Size.Fill(),
    Height = Size.Abs(headerHeight),
    Title = "Header",
    ThinBorders = true,
};
window.AddChild(header);

header.AddChild(new TextElement
{
    X = Pos.Center(),
    Y = Pos.Abs(1),
    Text = "Welcome to Scoria",
    Style = titleStyle,
});

header.AddChild(new TextElement
{
    X = Pos.Begin(1),
    Y = Pos.Abs(3),
    Text = "A console UI layout engine",
    Style = subtitleStyle,
});

header.AddChild(new TextElement
{
    X = Pos.End(1),
    Y = Pos.Abs(3),
    Text = "v0.1.0",
    Style = dimStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// SIDEBAR – below header, fixed width, height fits children
// ═══════════════════════════════════════════════════════════════════════
PanelElement sidebar = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.After(0, header),
    Width = Size.Abs(sidebarWidth),
    Height = Size.FitChildren(),
    Title = "Sidebar",
};
window.AddChild(sidebar);

sidebar.AddChild(new TextElement
{
    X = Pos.Begin(2),
    Y = Pos.Abs(1),
    Text = "Navigation",
    Style = labelStyle,
});

sidebar.AddChild(new TextElement
{
    X = Pos.Begin(2),
    Y = Pos.Abs(3),
    Text = "- Dashboard",
    Style = sidebarItemStyle,
});

sidebar.AddChild(new TextElement
{
    X = Pos.Begin(2),
    Y = Pos.Abs(5),
    Text = "- Settings",
    Style = sidebarItemStyle,
});

sidebar.AddChild(new TextElement
{
    X = Pos.Begin(2),
    Y = Pos.Abs(7),
    Text = "- Help",
    Style = sidebarItemStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// MAIN CONTENT – to the right of sidebar, below header
//   Requires Width/Height from Application to compute
//   (terminal width - sidebar, terminal height - header).
// ═══════════════════════════════════════════════════════════════════════
PanelElement main = new PanelElement
{
    X = Pos.After(0, sidebar),
    Y = Pos.After(0, header),
    Width = Size.Fill(sidebarWidth, window),
    Height = Size.Fill(headerHeight, window),
    Title = "Main Content",
};
window.AddChild(main);

// ═══════════════════════════════════════════════════════════════════════
// CARD 1 – Stats card
// ═══════════════════════════════════════════════════════════════════════
PanelElement card1 = new PanelElement
{
    X = Pos.Relative(0.05f, main),
    Y = Pos.Relative(0.05f, main),
    Width = Size.Abs(28),
    Height = Size.Abs(8),
    Title = "Stats",
};
main.AddChild(card1);

card1.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card1),
    Y = Pos.Relative(0.15f, card1),
    Text = "Active Users:",
    Style = labelStyle,
});

card1.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card1),
    Y = Pos.Relative(0.35f, card1),
    Text = "  42",
    Style = valueStyle,
});

card1.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card1),
    Y = Pos.Relative(0.6f, card1),
    Text = "Uptime:",
    Style = labelStyle,
});

card1.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card1),
    Y = Pos.Relative(0.8f, card1),
    Text = "  99.9%",
    Style = statusStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// CARD 2 – System card, positioned below card1
// ═══════════════════════════════════════════════════════════════════════
PanelElement card2 = new PanelElement
{
    X = Pos.Relative(0.05f, main),
    Y = Pos.After(1, card1),
    Width = Size.Abs(28),
    Height = Size.Abs(6),
    Title = "System",
};
main.AddChild(card2);

card2.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card2),
    Y = Pos.Relative(0.2f, card2),
    Text = "CPU:",
    Style = labelStyle,
});

card2.AddChild(new TextElement
{
    X = Pos.Relative(0.4f, card2),
    Y = Pos.Relative(0.2f, card2),
    Text = "[||||] 67%",
    Style = accentStyle,
});

card2.AddChild(new TextElement
{
    X = Pos.Relative(0.05f, card2),
    Y = Pos.Relative(0.65f, card2),
    Text = "Memory:",
    Style = labelStyle,
});

card2.AddChild(new TextElement
{
    X = Pos.Relative(0.4f, card2),
    Y = Pos.Relative(0.65f, card2),
    Text = "[||||||] 4.2 GB",
    Style = accentStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// SIDE PANEL – to the right of cards
// ═══════════════════════════════════════════════════════════════════════
PanelElement sidePanel = new PanelElement
{
    X = Pos.After(2, card1),
    Y = Pos.Relative(0.05f, main),
    Width = Size.Relative(0.4f, main),
    Height = Size.Abs(8),
    Title = "Info",
};
main.AddChild(sidePanel);

sidePanel.AddChild(new TextElement
{
    X = Pos.Center(),
    Y = Pos.Center(),
    Text = "Centered!",
    Style = new Style(255, 150, 255, 20, 20, 40, 255, StyleAttributes.Bold | StyleAttributes.Underline),
});

sidePanel.AddChild(new TextElement
{
    X = Pos.End(1),
    Y = Pos.Begin(1),
    Text = "Right-aligned",
    Style = dimStyle,
});

sidePanel.AddChild(new TextElement
{
    X = Pos.Begin(1),
    Y = Pos.End(1),
    Text = "Bottom-left corner",
    Style = dimStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// STATUS BAR – at the very bottom
// ═══════════════════════════════════════════════════════════════════════
PanelElement statusBar = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.Relative(1f, window),
    Width = Size.Fill(),
    Height = Size.Abs(1),
    Title = "",
};
window.AddChild(statusBar);

statusBar.AddChild(new TextElement
{
    X = Pos.Begin(1),
    Y = Pos.Abs(0),
    Text = "Ready",
    Style = statusStyle,
});

statusBar.AddChild(new TextElement
{
    X = Pos.End(1),
    Y = Pos.Abs(0),
    Text = "Scoria Layout Engine",
    Style = dimStyle,
});


// ═══════════════════════════════════════════════════════════════════════
// RUN
// ═══════════════════════════════════════════════════════════════════════
var app = new Application(new ApplicationOptions { Window = window });
window.Events.Add<MouseButtonEventArgs>(e =>
{
    if (e.Button == Button.Middle)
    {
        app.Stop();
    }
});
app.Start();
