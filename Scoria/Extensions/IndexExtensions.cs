internal static class IndexExtensions
{
    public static Index Add(this Index index, int value)
    {
        return new Index(index.Value + value, index.IsFromEnd);
    }
}
