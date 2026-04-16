using System.Collections.Concurrent;
using System.Windows.Media;

namespace BookingRoom.Client.WPF.Services.Themes;

public interface IThemeService
{
    Theme GetTheme(int seed);

    Brush GetColor(Theme theme, ThemeRole role, int level);

    Brush GetTextColor(Brush background);
}

public class ThemeService : IThemeService
{
    private readonly ConcurrentDictionary<int, Theme> _cache = new();

    public Theme GetTheme(int seed)
    {
        return _cache.GetOrAdd(seed, BuildTheme);
    }

    public Brush GetColor(Theme theme, ThemeRole role, int level)
    {
        if (!theme.Scales.TryGetValue(role, out var scale))
            return Brushes.Transparent;

        if (scale.TryGetValue(level, out var brush))
            return brush;

        // fallback → nearest level
        var nearest = scale.Keys.OrderBy(k => Math.Abs(k - level)).First();
        return scale[nearest];
    }

    public Brush GetTextColor(Brush background)
    {
        if (background is not SolidColorBrush scb)
            return Brushes.Black;

        var c = scb.Color;

        double luminance =
            (0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B) / 255;

        return luminance > 0.6
            ? new SolidColorBrush(Color.FromRgb(33, 33, 33))   // soft black
            : Brushes.White;
    }

    // =========================
    // BUILD THEME
    // =========================
    private Theme BuildTheme(int seed)
    {
        var baseColor = GenerateColorFromId(seed);

        ColorToHsl(baseColor, out double h, out double s, out _);

        var levels = BuildLevels();

        var theme = new Theme
        {
            Scales = new Dictionary<ThemeRole, Dictionary<int, Brush>>
            {
                [ThemeRole.Base] = new(),
                [ThemeRole.Accent] = new(),
                [ThemeRole.Surface] = new()
            }
        };

        foreach (var (level, lightness) in levels)
        {
            // =========================
            // BASE
            // =========================
            var baseC = HslToColor(
                h,
                ClampMin(s, 0.40),
                lightness
            );

            theme.Scales[ThemeRole.Base][level] =
                Freeze(new SolidColorBrush(baseC));

            // =========================
            // ACCENT
            // =========================
            var accentC = HslToColor(
                h,
                ClampMin(s * 1.3, 0.55),
                Clamp(lightness - 0.18, 0.25, 0.70)
            );

            theme.Scales[ThemeRole.Accent][level] =
                Freeze(new SolidColorBrush(accentC));

            // =========================
            // SURFACE (neutro leggibile)
            // =========================
            var surfaceC = HslToColor(
                h,
                ClampMin(s * 0.25, 0.10),
                Clamp(lightness + 0.12, 0.20, 0.95)
            );

            theme.Scales[ThemeRole.Surface][level] =
                Freeze(new SolidColorBrush(surfaceC));
        }

        return theme;
    }

    private static Dictionary<int, double> BuildLevels()
    {
        return new Dictionary<int, double>
        {
            [50] = 0.97,
            [100] = 0.93,
            [150] = 0.88,
            [200] = 0.83,
            [250] = 0.78,
            [300] = 0.72,
            [350] = 0.66,
            [400] = 0.60,
            [450] = 0.55,
            [500] = 0.50,
            [550] = 0.45,
            [600] = 0.40,
            [650] = 0.35,
            [700] = 0.30,
            [750] = 0.26,
            [800] = 0.22,
            [900] = 0.18
        };
    }

    // =========================
    // COLOR GENERATOR
    // =========================
    private static Color GenerateColorFromId(int id)
    {
        double hue = (id * 0.61803398875) % 1.0;

        double saturation = 0.60;
        double lightness = 0.55;

        return HslToColor(hue, saturation, lightness);
    }

    // =========================
    // UTILS
    // =========================
    private static SolidColorBrush Freeze(SolidColorBrush b)
    {
        b.Freeze();
        return b;
    }

    private static double Clamp(double v, double min, double max)
        => v < min ? min : (v > max ? max : v);

    private static double ClampMin(double v, double min)
        => v < min ? min : v;

    // =========================
    // HSL
    // =========================
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
