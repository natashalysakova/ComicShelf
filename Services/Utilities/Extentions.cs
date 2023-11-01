public static class Extentions
{
    const double MAX = 100.0;

    public static double NextDoublePercent(this Random random, int min = 0, int max = 100)
    {
        return min > max || min < uint.MinValue
            ? throw new ArgumentException($"Min should be between 0 and {max}")
            : max > MAX || max < uint.MinValue
            ? throw new ArgumentException($"Max should be between 0 and {MAX}")
            : random.Next(min, max) / MAX;
    }

    public static string Replace(this string s, char[] separators, string newVal)
    {
        return String.Join(newVal, s.Split(separators, StringSplitOptions.RemoveEmptyEntries));
    }

    public static bool ContainsIgnoreCase(this string source, string toCheck)
    {
        return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) != -1;
    }
}