using System.Text;
using Scoria;
using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Events;
using Scoria.Layout;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

ConsoleDriver driver = new ConsoleDriver(
    new FocusInputProvider(),
    new KeyInputProvider(),
    new MouseInputProvider(),
    new PasteInputProvider(),
    new ResizeInputProvider()
);

driver.OnEvent += eventArgs =>
{
    if (eventArgs is MouseButtonEventArgs args && args.Button == Button.Middle)
    {
        Environment.Exit(0);
    }
};

Surface surface = new Surface(driver.Width, driver.Height);
surface.Fill(' ', new Style(255, 255, 255, 20, 20, 30));

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
// ROOT – sized to the terminal dimensions
//   Pos.Abs(0)              – fixed origin at (0, 0)
//   Size.Abs(driver.Width)  – fixed terminal width
//   Size.Abs(driver.Height) – fixed terminal height
// ═══════════════════════════════════════════════════════════════════════
PanelElement root = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.Abs(0),
    Width = Size.Abs(driver.Width),
    Height = Size.Abs(driver.Height),
    Title = "Scoria Layout Demo",
};

// ═══════════════════════════════════════════════════════════════════════
// HEADER – fixed 5 rows tall, fills parent width
//   Size.Abs(5)  – exactly 5 rows
//   Size.Fill()  – full width of root (no offset, so no overflow)
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
root.AddChild(header);

// Pos.Center()  – horizontally centered within header
// Pos.Abs(1)    – 1 row from top of header
header.AddChild(new TextElement
{
    X = Pos.Center(),
    Y = Pos.Abs(1),
    Text = "Welcome to Scoria",
    Style = titleStyle,
});

// Pos.Begin()   – left edge of header (Relative(0))
// Pos.Abs(3)    – 3 rows from top
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
// SIDEBAR – below header, fixed width
//   Pos.Abs(0)            – left edge
//   Pos.After(0, header)  – immediately below header
//   Size.Abs(24)      – 24 columns wide
//   Size.FitChildren() – height matches the sum of its children's heights
// ═══════════════════════════════════════════════════════════════════════
PanelElement sidebar = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.After(0, header),
    Width = Size.Abs(sidebarWidth),
    Height = Size.FitChildren(),
    Title = "Sidebar",
};
root.AddChild(sidebar);

// Abs positioning for Y avoids circular dependency with FitChildren on sidebar height.
// The bounding box of these children determines the sidebar's height.
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
//   Both axes have offsets, so Size.Fill() would overflow on both.
//   Use explicit sizes: width = terminal - sidebar, height = terminal - header.
// ═══════════════════════════════════════════════════════════════════════
PanelElement main = new PanelElement
{
    X = Pos.After(0, sidebar),
    Y = Pos.After(0, header),
    Width = Size.Abs(driver.Width - sidebarWidth),
    Height = Size.Abs(driver.Height - headerHeight),
    Title = "Main Content",
};
root.AddChild(main);

// ═══════════════════════════════════════════════════════════════════════
// CARD 1 – Stats card with fixed size, positioned with Relative
//   Pos.Relative(0.05f, main)  – 5% from main's left/top edge
//   Size.Abs(28) x Size.Abs(8) – fixed 28×8 box
//   Children use Relative() to spread across the card area
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

// Relative positioning distributes content across the card's 28×8 area
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
// CARD 2 – System card, positioned below card1 via After()
//   Pos.Relative(0.05f, main)  – same X as card1
//   Pos.After(1, card1)        – 1 row below card1's bottom edge
//   Size.Abs(28) x Size.Abs(6)
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
// SIDE PANEL – to the right of cards, fixed dimensions
//   Pos.After(2, card1)   – 2 columns right of card1's right edge
//   Size.Relative(0.4f)   – 40% of main's width
//   Size.Abs(8)           – fixed 8 rows tall
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

// Pos.Center() – centered horizontally within sidePanel
// Pos.Center() – centered vertically within sidePanel
sidePanel.AddChild(new TextElement
{
    X = Pos.Center(),
    Y = Pos.Center(),
    Text = "Centered!",
    Style = new Style(255, 150, 255, 20, 20, 40, 255, StyleAttributes.Bold | StyleAttributes.Underline),
});

// Pos.End(1)   – right edge with 1-char padding
// Pos.Begin(1) – top edge with 1-char padding
sidePanel.AddChild(new TextElement
{
    X = Pos.End(1),
    Y = Pos.Begin(1),
    Text = "Right-aligned",
    Style = dimStyle,
});

// Pos.Begin(1) – left edge with 1-char padding
// Pos.End(1)   – bottom edge with 1-char padding
sidePanel.AddChild(new TextElement
{
    X = Pos.Begin(1),
    Y = Pos.End(1),
    Text = "Bottom-left corner",
    Style = dimStyle,
});

// ═══════════════════════════════════════════════════════════════════════
// STATUS BAR – at the very bottom of root
//   Pos.Relative(1f, root)  – End of root (bottom edge)
//   Relative(1f) = (root.Height - selfHeight) * 1 + root.Y
//   With root.Y=0 and selfHeight=1: Y = root.Height - 1 (last row)
//   Size.Fill() is safe here: X=Abs(0), so no horizontal overflow.
// ═══════════════════════════════════════════════════════════════════════
PanelElement statusBar = new PanelElement
{
    X = Pos.Abs(0),
    Y = Pos.Relative(1f, root),
    Width = Size.Fill(),
    Height = Size.Abs(1),
    Title = "",
};
root.AddChild(statusBar);

statusBar.AddChild(new TextElement
{
    X = Pos.Begin(1),
    Y = Pos.Abs(0),
    Text = "Ready | Middle-click to exit",
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
// SOLVE – resolve all layout properties via topological sort
//   includeSelf: true  – solve root's own layout too
// ═══════════════════════════════════════════════════════════════════════
LayoutSolver.Solve(root, true);

// ═══════════════════════════════════════════════════════════════════════
// RENDER LOOP
// ═══════════════════════════════════════════════════════════════════════
while (true)
{
    root.Render(surface);
    surface.ExpandBorders();
    driver.Frame(surface);
    driver.PollInput();
}
