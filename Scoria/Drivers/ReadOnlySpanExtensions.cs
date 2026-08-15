namespace Scoria.Drivers;

internal static class ReadOnlySpanExtensions
{
    /// <summary>Reads an unsigned decimal number at the current position, advancing past it.</summary>
    /// <param name="input">The span to read from.</param>
    /// <param name="pos">The position to read at; advanced past the digits on success.</param>
    /// <param name="value">The parsed number.</param>
    /// <returns><see langword="true"/> if at least one digit was read; otherwise <see langword="false"/>.</returns>
    public static bool TryReadNumber(this ReadOnlySpan<char> input, ref int pos, out int value)
    {
        value = 0;
        int start = pos;
        while (pos < input.Length && char.IsAsciiDigit(input[pos]))
        {
            value = value * 10 + (input[pos] - '0');
            pos++;
        }
        return pos > start;
    }

    /// <summary>Reads the specified text at the current position, advancing past it.</summary>
    /// <param name="input">The span to read from.</param>
    /// <param name="pos">The position to read at; advanced past the text on success.</param>
    /// <param name="expected">The text to match.</param>
    /// <returns><see langword="true"/> if the text was present; otherwise <see langword="false"/>.</returns>
    public static bool TryReadString(this ReadOnlySpan<char> input, ref int pos, ReadOnlySpan<char> expected)
    {
        if ((uint)pos > (uint)input.Length || !input[pos..].StartsWith(expected))
        {
            return false;
        }

        pos += expected.Length;
        return true;
    }
}
