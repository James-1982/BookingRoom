using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookingRoom.Client.WPF.Converter;

public class DarkenBrushConverter : IValueConverter
{
    // 0.0 = nero, 1.0 = originale
    public double LightnessFactor { get; set; } = 0.75;

    public double SaturationBoost { get; set; } = 1.15;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
            return value;

        var c = brush.Color;

        ColorToHsl(c, out double h, out double s, out double l);

        l *= LightnessFactor;
        s *= SaturationBoost;

        s = Clamp01(s);
        l = Clamp01(l);

        var result = HslToColor(h, s, l);

        return new SolidColorBrush(result);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static double Clamp01(double v) => Math.Max(0, Math.Min(1, v));

    private static void ColorToHsl(Color c, out double h, out double s, out double l)
    {
        double r = c.R / 255.0;
        double g = c.G / 255.0;
        double b = c.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        l = (max + min) / 2;

        if (max == min)
        {
            h = s = 0;
            return;
        }

        double d = max - min;

        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

        if (max == r)
            h = (g - b) / d + (g < b ? 6 : 0);
        else if (max == g)
            h = (b - r) / d + 2;
        else
            h = (r - g) / d + 4;

        h /= 6;
    }

    private static Color HslToColor(double h, double s, double l)
    {
        double r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;

            r = HueToRgb(p, q, h + 1.0 / 3);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3);
        }

        return Color.FromRgb(
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255));
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2) return q;
        if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        return p;
    }
}
