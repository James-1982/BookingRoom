using BookingRoom.Client.WPF.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BookingRoom.Client.WPF.Views;

public partial class DayBookingsContainer : UserControl
{
    public DayBookingsContainer()
    {
        InitializeComponent();

        Loaded += (_, __) =>
        {
            if (DataContext is DayBookingsContainerViewModel vm)
            {
                vm.AnimationRequested += PlayRoomChangeAnimation;
            }
        };
    }

    public void PlayRoomChangeAnimation()
    {
        AnimateOut(() =>
        {
            AnimateIn();
        });
    }

    private void AnimateOut(Action completed)
    {
        var fade = new DoubleAnimation
        {
            To = 0.2,
            Duration = TimeSpan.FromMilliseconds(120)
        };

        var slide = new DoubleAnimation
        {
            To = -40,
            Duration = TimeSpan.FromMilliseconds(120)
        };

        fade.Completed += (_, __) => completed?.Invoke();

        Root.BeginAnimation(OpacityProperty, fade);
        SlideTransform.BeginAnimation(TranslateTransform.XProperty, slide);
    }

    private void AnimateIn()
    {
        var fade = new DoubleAnimation
        {
            To = 1,
            Duration = TimeSpan.FromMilliseconds(220),
            EasingFunction = new CubicEase()
        };

        var slide = new DoubleAnimation
        {
            From = 40,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(220),
            EasingFunction = new CubicEase()
        };

        Root.BeginAnimation(OpacityProperty, fade);
        SlideTransform.BeginAnimation(TranslateTransform.XProperty, slide);
    }
}