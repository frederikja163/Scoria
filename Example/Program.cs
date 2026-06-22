using Scoria;

Style white = new Style(0, 0, 0, 255, 255, 255);
Style black = new Style(255, 255, 255, 0, 0, 0);

int w = Console.WindowWidth;
int h = Console.WindowHeight;

Surface checkeredX = new Surface(w, h);
Surface checkeredY = new Surface(w, h);
for (int i = 0; i < checkeredX.Width; i++)
{
    for (int j = 0; j < checkeredX.Height; j++)
    {
        checkeredX.Write((char)('0' + i % 10), i, j, (i + j) % 2 == 0 ? white : black);
        checkeredY.Write((char)('0' + j % 10), i, j, (i + j) % 2 == 0 ? white : black);
    }
}

Surface blue = new Surface(5, 5);
blue.Fill('#', new Style(20, 20, 20, 60, 80, 200, 50));
blue.Fill('.', 1, 1, 3, 3, new Style(20, 20, 20, 60, 80, 200));

Surface screen = new Surface(w, h);
int toggle = 0;

while (true)
{
    screen.Fill(' ', default);
    screen.Write(toggle == 0 ? checkeredX : checkeredY, 0, 0);
    screen.Write(blue, toggle == 0 ? -2 : 2, toggle == 0 ? 0 : 27);
    ConsoleDriver.Frame(screen);

    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
        break;

    toggle ^= 1;
}
