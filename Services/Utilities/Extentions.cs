public static class Extentions
{
    

    public static double NextDoublePercent(this Random random, int min = 0, int max = 100)
    {
        if (min > max || min < 0)
            throw new ArgumentException("Min should be between 0 and max");

        if (max > 100 || max < 0)
            throw new ArgumentException("Max should be between 0 and 100");


        return random.Next(min, max) / 100.0;
    }

    public static string Replace(this string s, char[] separators, string newVal)
    {
        string[] temp;

        temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        return String.Join(newVal, temp);
    }

    public static bool ContainsIgnoreCase(this string source, string toCheck)
    {
        return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) != -1;
    }
}