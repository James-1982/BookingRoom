using System.Windows;

namespace BookingRoom.Client.WPF.Behaviors;

public static class WatermarkBehavior
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(WatermarkBehavior),
            new PropertyMetadata(string.Empty));

    public static void SetText(DependencyObject element, string value)
        => element.SetValue(TextProperty, value);

    public static string GetText(DependencyObject element)
        => (string)element.GetValue(TextProperty);
}