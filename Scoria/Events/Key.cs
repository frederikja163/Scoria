namespace Scoria.Events;

/// <summary>Represents a keyboard key, including modifiers and unmapped unicode characters.</summary>
[Flags]
public enum Key
{
    /// <summary>No key pressed.</summary>
    None = 0,

    /// <summary>The A key.</summary>
    A,
    /// <summary>The B key.</summary>
    B,
    /// <summary>The C key.</summary>
    C,
    /// <summary>The D key.</summary>
    D,
    /// <summary>The E key.</summary>
    E,
    /// <summary>The F key.</summary>
    F,
    /// <summary>The G key.</summary>
    G,
    /// <summary>The H key.</summary>
    H,
    /// <summary>The I key.</summary>
    I,
    /// <summary>The J key.</summary>
    J,
    /// <summary>The K key.</summary>
    K,
    /// <summary>The L key.</summary>
    L,
    /// <summary>The M key.</summary>
    M,
    /// <summary>The N key.</summary>
    N,
    /// <summary>The O key.</summary>
    O,
    /// <summary>The P key.</summary>
    P,
    /// <summary>The Q key.</summary>
    Q,
    /// <summary>The R key.</summary>
    R,
    /// <summary>The S key.</summary>
    S,
    /// <summary>The T key.</summary>
    T,
    /// <summary>The U key.</summary>
    U,
    /// <summary>The V key.</summary>
    V,
    /// <summary>The W key.</summary>
    W,
    /// <summary>The X key.</summary>
    X,
    /// <summary>The Y key.</summary>
    Y,
    /// <summary>The Z key.</summary>
    Z,

    /// <summary>The 0 key.</summary>
    D0,
    /// <summary>The 1 key.</summary>
    D1,
    /// <summary>The 2 key.</summary>
    D2,
    /// <summary>The 3 key.</summary>
    D3,
    /// <summary>The 4 key.</summary>
    D4,
    /// <summary>The 5 key.</summary>
    D5,
    /// <summary>The 6 key.</summary>
    D6,
    /// <summary>The 7 key.</summary>
    D7,
    /// <summary>The 8 key.</summary>
    D8,
    /// <summary>The 9 key.</summary>
    D9,

    /// <summary>The Space bar.</summary>
    Space,
    /// <summary>The Enter key.</summary>
    Enter,
    /// <summary>The Tab key.</summary>
    Tab,
    /// <summary>The Escape key.</summary>
    Escape,
    /// <summary>The Backspace key.</summary>
    Backspace,

    /// <summary>The Up arrow key.</summary>
    Up,
    /// <summary>The Down arrow key.</summary>
    Down,
    /// <summary>The Left arrow key.</summary>
    Left,
    /// <summary>The Right arrow key.</summary>
    Right,
    /// <summary>The Home key.</summary>
    Home,
    /// <summary>The End key.</summary>
    End,
    /// <summary>The Page Up key.</summary>
    PageUp,
    /// <summary>The Page Down key.</summary>
    PageDown,
    /// <summary>The Insert key.</summary>
    Insert,
    /// <summary>The Delete key.</summary>
    Delete,

    /// <summary>The F1 key.</summary>
    F1,
    /// <summary>The F2 key.</summary>
    F2,
    /// <summary>The F3 key.</summary>
    F3,
    /// <summary>The F4 key.</summary>
    F4,
    /// <summary>The F5 key.</summary>
    F5,
    /// <summary>The F6 key.</summary>
    F6,
    /// <summary>The F7 key.</summary>
    F7,
    /// <summary>The F8 key.</summary>
    F8,
    /// <summary>The F9 key.</summary>
    F9,
    /// <summary>The F10 key.</summary>
    F10,
    /// <summary>The F11 key.</summary>
    F11,
    /// <summary>The F12 key.</summary>
    F12,

    /// <summary>A unicode character with no named key. The character is provided by <see cref="Key.ToKey"/>.</summary>
    Unicode = 0x8000,

    /// <summary>The Control modifier.</summary>
    Ctrl  = 0x1000,
    /// <summary>The Alt modifier.</summary>
    Alt   = 0x2000,
    /// <summary>The Shift modifier.</summary>
    Shift = 0x4000,
}

/// <summary>Extension methods for the <see cref="Key"/> enum.</summary>
public static class KeyExtensions
{
    private const Key BaseMask = (Key)0x0FFF;

    /// <summary>Strips modifier flags, returning only the base key.</summary>
    /// <param name="key">The key to strip modifiers from.</param>
    /// <returns>The key with all modifier flags removed.</returns>
    public static Key Base(this Key key) => key & BaseMask;

    extension (Key)
    {
        /// <summary>Converts a character to its corresponding <see cref="Key"/>.</summary>
        /// <param name="c">The character to convert.</param>
        /// <param name="ch">
        /// When this method returns, contains the character associated with the key,
        /// or <see langword="null"/> for control keys with no character representation.
        /// </param>
        /// <returns>
        /// The <see cref="Key"/> matching the character, or <see cref="Key.Unicode"/>
        /// if no named key maps to the character.
        /// </returns>
        public static Key ToKey(char c, out char? ch)
        {
            (Key key, ch) = c switch
            {
                >= 'a' and <= 'z' => ((Key)(c - 'a' + (int)Key.A), (char?)c),
                >= 'A' and <= 'Z' => ((Key)(c - 'A' + (int)Key.A), (char?)c),
                >= '0' and <= '9' => ((Key)(c - '0' + (int)Key.D0), (char?)c),
                ' '               => (Key.Space, (char?)' '),
                '\r' or '\n'      => (Key.Enter, '\n'),
                '\t'              => (Key.Tab, '\t'),
                '\x1B'            => (Key.Escape, null),
                '\x7F' or '\x08'  => (Key.Backspace, null),
                _                 => (Key.Unicode, (char?)c),
            };
            return key;
        }
    }
}
