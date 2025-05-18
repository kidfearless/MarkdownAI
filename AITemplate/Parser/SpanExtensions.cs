namespace AITemplate.Parser;

public static class SpanExtensions
{
    /// <summary>
    /// Checks if a ReadOnlySpan<char> consists only of whitespace characters
    /// </summary>
    public static bool IsWhiteSpace(this ReadOnlySpan<char> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsWhiteSpace(span[i]))
            {
                return false;
            }
        }
        return true;
    }
}
