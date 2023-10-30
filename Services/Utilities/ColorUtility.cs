using System.Drawing;

public static class ColorUtility
{
    private static Random random;

    static Random Random
    {
        get
        {
            if (random == null)
                random = new Random();
            return random;
        }
    }

    public static Color GetRandomColor()
    {
        return Color.FromArgb(Random.Next(0, 255), Random.Next(0, 255), Random.Next(0, 255));
    }
    public static Color GetRandomColor(
        int minHue = 0, int maxHue = 360,
        int minSaturation = 0, int maxSaturation = 100,
        int minValue = 0, int maxValue = 100)
    {
        var hue = Random.Next(minHue, maxHue);
        var saturation = Random.NextDoublePercent(minSaturation, maxSaturation);
        var value = Random.NextDoublePercent(minValue, maxValue);

        return ColorFromHSV(hue, saturation, value);
    }

    public static Color GetOppositeColor(Color original)
    {
        ColorToHSV(original, out double hue, out double saturation, out double value);

        hue = (hue + 180) % 360;

        return ColorFromHSV(hue, saturation, value);
    }

    public static String HexConverter(Color c)
    {
        return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
    }

    private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
    {
        int max = Math.Max(color.R, Math.Max(color.G, color.B));
        int min = Math.Min(color.R, Math.Min(color.G, color.B));

        hue = color.GetHue();
        saturation = (max == 0) ? 0 : 1d - (1d * min / max);
        value = max / 255d;
    }

    private static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return Color.FromArgb(255, v, t, p);
        else if (hi == 1)
            return Color.FromArgb(255, q, v, p);
        else if (hi == 2)
            return Color.FromArgb(255, p, v, t);
        else if (hi == 3)
            return Color.FromArgb(255, p, q, v);
        else if (hi == 4)
            return Color.FromArgb(255, t, p, v);
        else
            return Color.FromArgb(255, v, p, q);
    }

    internal static Color ChangeSaturation(Color color, int percent)
    {
        ColorToHSV(color, out double hue, out double saturation, out double value);

        saturation += (percent / 100.0);
        if (saturation > 1)
            saturation = 1;
        if (saturation < 0)
            saturation = 0;

        return ColorFromHSV(hue, saturation, value);
    }
    internal static Color ChangeValue(Color color, int percent)
    {
        ColorToHSV(color, out double hue, out double saturation, out double value);

        value += (percent / 100.0);
        if (value > 1)
            value = 1;
        if (value < 0)
            value = 0;

        return ColorFromHSV(hue, saturation, value);
    }
}
