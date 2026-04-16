using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using BookingRoom.Client.WPF.Utils;
using BookingRoom.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace BookingRoom.Client.WPF.ViewModels;

public partial class BookingViewModel : ObservableObject
{
    private readonly BookingDto _dto;
    private readonly ITimeService _timeService;
    private readonly IThemeService _themeService;
    private readonly Theme _theme;

    public BookingViewModel(BookingDto dto, Theme roomTheme, ITimeService timeService, IThemeService themeService)
    {
        _dto = dto;
        _theme = roomTheme;
        _timeService = timeService;
        _themeService = themeService;

        Title = _dto.Title;
        Start = _dto.StartTime;
        End = _dto.EndTime;

        UpdateState();

        _timeService.Tick += OnTick;
    }


    [ObservableProperty]
    private bool _isMouseOver;

    partial void OnIsMouseOverChanged(bool value)
    {
        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(AccentColor));
        OnPropertyChanged(nameof(TextColor));
    }

    [ObservableProperty]
    private DateTime _start;

    [ObservableProperty]
    private DateTime _end;

    public bool IsEnabled { get; set; } = true;

    public string Title { get; set; } = string.Empty;

    public Density Density { get; set; } = Density.Normal;

    public BookingState State
    {
        get
        {
            var now = _timeService.Now;

            if (End < now)
                return BookingState.Past;

            if (!IsEnabled)
                return BookingState.Disabled;

            if (Start <= now && End >= now)
                return BookingState.Active;

            if (IsMouseOver)
                return BookingState.Hover;

            return BookingState.Default;
        }
    }

    public DateTime StartTime => _dto.StartTime;
    public DateTime EndTime => _dto.EndTime;

    public string TimeRange => $"{StartTime:HH:mm} - {EndTime:HH:mm}";

    public bool IsRunning => StartTime <= _timeService.Now && EndTime >= _timeService.Now;

    public bool IsExpired => EndTime < _timeService.Now;

    public Brush BackgroundColor => State switch
    {
        BookingState.Active => _theme.Scales[ThemeRole.Base][500],
        BookingState.Hover => _theme.Scales[ThemeRole.Base][400],
        BookingState.Past => _theme.Scales[ThemeRole.Base][100],
        BookingState.Disabled => _theme.Scales[ThemeRole.Surface][150],

        _ => _theme.Scales[ThemeRole.Base][GetVariantLevel(_dto.Id, 350)]
    };

    public Brush AccentColor => State switch
    {
        BookingState.Active => _theme.Scales[ThemeRole.Accent][550],
        BookingState.Hover => _theme.Scales[ThemeRole.Accent][450],
        BookingState.Past => _theme.Scales[ThemeRole.Accent][150],
        BookingState.Disabled => _theme.Scales[ThemeRole.Surface][300],

        _ => _theme.Scales[ThemeRole.Accent][GetVariantLevel(_dto.Id, 400)]
    };

    public Brush TextColor => _themeService.GetTextColor(BackgroundColor);

    private void OnTick()
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(IsRunning));
            OnPropertyChanged(nameof(IsExpired));
            OnPropertyChanged(nameof(BackgroundColor));
            OnPropertyChanged(nameof(AccentColor));
        });
    }

    private void UpdateState()
    {
        OnPropertyChanged(nameof(State));
        OnPropertyChanged(nameof(IsRunning));
        OnPropertyChanged(nameof(IsExpired));
        OnPropertyChanged(nameof(BackgroundColor));
        OnPropertyChanged(nameof(AccentColor));
    }

    private static int GetVariantLevel(int id, int baseLevel)
    {
        int[] offsets = { -50, 0, 50 }; // 👈 micro variazione safe

        int offset = offsets[Math.Abs(id) % offsets.Length];

        return baseLevel + offset;
    }
}