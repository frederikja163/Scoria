using Scoria;

Style style1 = new Style(80, 20, 250, StyleAttributes.None);
Style style2 = new Style(255, 20, 250, StyleAttributes.Bold | StyleAttributes.DoubleUnderline);

Style white = new Style(0, 0, 0, 255, 255, 255);
Style black = new Style(255, 255, 255, 0, 0, 0);

Surface checkeredX = new Surface(Console.BufferWidth, Console.BufferHeight);
Surface checkeredY = new Surface(Console.BufferWidth, Console.BufferHeight);
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

Surface screen;

while (true)
{
    screen = new Surface(Console.BufferWidth, Console.BufferHeight);
    screen.Write(checkeredX, 0, 0);
    screen.Write(blue, -2, 0);
    ConsoleDriver.Frame(screen);
    Console.ReadKey();
    screen = new Surface(Console.BufferWidth, Console.BufferHeight);
    screen.Write(checkeredY, 0, 0);
    screen.Write(blue, 2, 27);
    ConsoleDriver.Frame(screen);
    Console.Read();
}