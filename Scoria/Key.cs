namespace Scoria;

/// <summary>Defines keyboard key codes, including control characters, digits, letters, symbols, and special/function keys.</summary>
public enum Key
{
    // Control characters
    /// <summary>The Tab key.</summary>
    Tab = 9,
    /// <summary>The Enter key.</summary>
    Enter = 13,
    /// <summary>The Escape key.</summary>
    Escape = 27,
    /// <summary>The Space bar.</summary>
    Space = 32,
    /// <summary>The Backspace key.</summary>
    Backspace = 127,

    // Digits
    /// <summary>The 0 key.</summary>
    D0 = '0',
    /// <summary>The 1 key.</summary>
    D1 = '1',
    /// <summary>The 2 key.</summary>
    D2 = '2',
    /// <summary>The 3 key.</summary>
    D3 = '3',
    /// <summary>The 4 key.</summary>
    D4 = '4',
    /// <summary>The 5 key.</summary>
    D5 = '5',
    /// <summary>The 6 key.</summary>
    D6 = '6',
    /// <summary>The 7 key.</summary>
    D7 = '7',
    /// <summary>The 8 key.</summary>
    D8 = '8',
    /// <summary>The 9 key.</summary>
    D9 = '9',

    // Letters
    /// <summary>The A key.</summary>
    A = 'A',
    /// <summary>The B key.</summary>
    B = 'B',
    /// <summary>The C key.</summary>
    C = 'C',
    /// <summary>The D key.</summary>
    D = 'D',
    /// <summary>The E key.</summary>
    E = 'E',
    /// <summary>The F key.</summary>
    F = 'F',
    /// <summary>The G key.</summary>
    G = 'G',
    /// <summary>The H key.</summary>
    H = 'H',
    /// <summary>The I key.</summary>
    I = 'I',
    /// <summary>The J key.</summary>
    J = 'J',
    /// <summary>The K key.</summary>
    K = 'K',
    /// <summary>The L key.</summary>
    L = 'L',
    /// <summary>The M key.</summary>
    M = 'M',
    /// <summary>The N key.</summary>
    N = 'N',
    /// <summary>The O key.</summary>
    O = 'O',
    /// <summary>The P key.</summary>
    P = 'P',
    /// <summary>The Q key.</summary>
    Q = 'Q',
    /// <summary>The R key.</summary>
    R = 'R',
    /// <summary>The S key.</summary>
    S = 'S',
    /// <summary>The T key.</summary>
    T = 'T',
    /// <summary>The U key.</summary>
    U = 'U',
    /// <summary>The V key.</summary>
    V = 'V',
    /// <summary>The W key.</summary>
    W = 'W',
    /// <summary>The X key.</summary>
    X = 'X',
    /// <summary>The Y key.</summary>
    Y = 'Y',
    /// <summary>The Z key.</summary>
    Z = 'Z',

    // Symbols
    /// <summary>The ! key.</summary>
    Exclaim = '!',
    /// <summary>The " key.</summary>
    DoubleQuote = '"',
    /// <summary>The # key.</summary>
    Hash = '#',
    /// <summary>The $ key.</summary>
    Dollar = '$',
    /// <summary>The % key.</summary>
    Percent = '%',
    /// <summary>The &amp; key.</summary>
    Amp = '&',
    /// <summary>The ' key.</summary>
    Quote = '\'',
    /// <summary>The ( key.</summary>
    LParen = '(',
    /// <summary>The ) key.</summary>
    RParen = ')',
    /// <summary>The * key.</summary>
    Star = '*',
    /// <summary>The + key.</summary>
    Plus = '+',
    /// <summary>The , key.</summary>
    Comma = ',',
    /// <summary>The - key.</summary>
    Minus = '-',
    /// <summary>The . key.</summary>
    Period = '.',
    /// <summary>The / key.</summary>
    Slash = '/',
    /// <summary>The : key.</summary>
    Colon = ':',
    /// <summary>The ; key.</summary>
    Semicolon = ';',
    /// <summary>The &lt; key.</summary>
    Lt = '<',
    /// <summary>The = key.</summary>
    Equal = '=',
    /// <summary>The &gt; key.</summary>
    Gt = '>',
    /// <summary>The ? key.</summary>
    Question = '?',
    /// <summary>The @ key.</summary>
    At = '@',
    /// <summary>The [ key.</summary>
    LBracket = '[',
    /// <summary>The \ key.</summary>
    Backslash = '\\',
    /// <summary>The ] key.</summary>
    RBracket = ']',
    /// <summary>The ^ key.</summary>
    Caret = '^',
    /// <summary>The _ key.</summary>
    Underscore = '_',
    /// <summary>The ` key.</summary>
    Backtick = '`',
    /// <summary>The { key.</summary>
    LBrace = '{',
    /// <summary>The | key.</summary>
    Pipe = '|',
    /// <summary>The } key.</summary>
    RBrace = '}',
    /// <summary>The ~ key.</summary>
    Tilde = '~',

    // Special / function keys (values intentionally above Unicode range)
    /// <summary>The Up arrow key.</summary>
    Up = 0x10000,
    /// <summary>The Down arrow key.</summary>
    Down,
    /// <summary>The Left arrow key.</summary>
    Left,
    /// <summary>The Right arrow key.</summary>
    Right,
    /// <summary>The Page Up key.</summary>
    PageUp,
    /// <summary>The Page Down key.</summary>
    PageDown,
    /// <summary>The Home key.</summary>
    Home,
    /// <summary>The End key.</summary>
    End,
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
}