namespace Scoria;

public sealed class Borders : IBorders
{
    private readonly char[] _borderCharacters =
    [
        ' ', '╶', '╺', ' ', '╵', '└', '┕', ' ', '╹', '┖', '┗', ' ', ' ', ' ', ' ', ' ', '╴', '─',
        '╼', ' ', '┘', '┴', '┶', ' ', '┚', '┸', '┺', ' ', ' ', ' ', ' ', ' ', '╸', '╾', '━', ' ',
        '┙', '┵', '┷', ' ', '┛', '┹', '┻', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '╷', '┌', '┍', ' ', '│', '├', '┝', ' ',
        '╿', '┞', '┡', ' ', ' ', ' ', ' ', ' ', '┐', '┬', '┮', ' ', '┤', '┼', '┾', ' ', '┦', '╀',
        '╄', ' ', ' ', ' ', ' ', ' ', '┑', '┭', '┯', ' ', '┥', '┽', '┿', ' ', '┩', '╃', '╇', ' ',
        ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ',
        ' ', ' ', '╻', '┎', '┏', ' ', '╽', '┟', '┢', ' ', '┃', '┠', '┣', ' ', ' ', ' ', ' ', ' ',
        '┒', '┰', '┲', ' ', '┧', '╁', '╆', ' ', '┨', '╂', '╊', ' ', ' ', ' ', ' ', ' ', '┓', '┱',
        '┳', ' ', '┪', '╅', '╈', ' ', '┫', '╉', '╋',
    ];
    
    public static char ThinBorderCharacter { get; } = '\x1e';
    public static char ThickBorderCharacter { get; } = '\x1f';
    
    public void ExpandBorders(ISurface surface)
    {
        byte[,] values = GetValues(surface);
        for (int y = 0; y < surface.Height; y++)
        {
            for (int x = 0; x < surface.Width; x++)
            {
                if (values[x, y] != 0)
                {
                    char c = GetChar(values, x, y);
                    surface.Write(c, x, y, surface.GetStyle(x, y));
                }
            }
        }
    }

    public void WriteTitle(ISurface surface, string title)
    {
        // TODO Handle cases where the title is too long.
        for (int i = 0; i < title.Length; i++)
        {
            surface.Write(title[i], 3 + i, 0, Theme.CurrentTheme.Title);
        }
    }

    private byte[,] GetValues(ISurface surface)
    {
        byte[,] values = new byte[surface.Width, surface.Height];
        for (int y = 0; y < surface.Height; y++)
        {
            for (int x = 0; x < surface.Width; x++)
            {
                if (surface.GetChar(x, y) == ThickBorderCharacter)
                {
                    values[x, y] = 2;
                }
                else if (surface.GetChar(x, y) == ThinBorderCharacter)
                {
                    values[x, y] = 1;
                }
            }
        }

        return values;
    }

    private char GetChar(byte[,] values, int x, int y)
    {
        byte ownValue = values[x, y];
        int width = values.GetLength(0);
        int height = values.GetLength(1);
        int charIndex = GetValue(1, 0) + (GetValue(0, -1) << 2) + (GetValue(-1, 0) << 4) + (GetValue(0, 1) << 6);

        return _borderCharacters[charIndex];

        int GetValue(int dx, int dy)
        {
            int value = (uint)(x + dx) < width && (uint)(y + dy) < height ? values[x + dx, y + dy] : 0;
            return value + ownValue == 3 ? 1 : value;
        }
    }
}