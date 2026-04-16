using System.Windows.Media;

namespace BookingRoom.Client.WPF.Services.Themes;

public class Theme
{
    public Dictionary<ThemeRole, Dictionary<int, Brush>> Scales { get; set; } = new();
}
