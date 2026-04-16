using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using BookingRoom.Client.WPF.Utils;
using BookingRoom.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BookingRoom.Client.WPF.ViewModels;

public partial class RoomViewModel : ObservableObject
{
    
    private readonly IBookingService _service;
    private readonly ITimeService _timeService;
    private readonly IThemeService _themeService;
    private readonly Dictionary<int, bool> _expiryCache = [];

    private int _activeCount;
    private int _expiredCount;
    public readonly Theme Theme;
    public RoomDto Room { get; }

    [ObservableProperty]
    private ObservableCollection<BookingDto> bookings = [];
    private Brush _textColor;

    public int BookingCount => _activeCount;

    public int BookingExpiredCount => _expiredCount;

    public Brush BaseColor => Theme.Scales[ThemeRole.Base][300];

    public Brush AccentColor => Theme.Scales[ThemeRole.Accent][400];

    public Brush TextColor => _textColor;

    public RoomViewModel(
        RoomDto room, 
        IBookingService service,
        ITimeService timeService,
        IThemeService themeService)
    {
        Room = room ?? throw new ArgumentNullException(nameof(room));

        Theme = themeService.GetTheme(room.Id);

        _textColor = themeService.GetTextColor(BaseColor);

        _service = service ?? throw new ArgumentNullException(nameof(service));

        _timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

        _ = LoadBookingsAsync();

        _timeService.Tick += TimeService_Tick;
    }

    private void TimeService_Tick()
    {
        foreach (var b in Bookings)
        {
            bool wasExpired = _expiryCache.TryGetValue(b.Id, out var old) && old;
            bool isExpired = b.IsExpired();

            if (wasExpired == isExpired)
                continue;

            if (!wasExpired && isExpired)
            {
                _expiredCount++;
                _activeCount--;
            }
            else if (wasExpired && !isExpired)
            {
                _expiredCount--;
                _activeCount++;
            }

            _expiryCache[b.Id] = isExpired;
        }
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        OnPropertyChanged(nameof(BookingCount));
        OnPropertyChanged(nameof(BookingExpiredCount));
    }

    [RelayCommand]
    private async Task LoadBookingsAsync()
    {
        try
        {
            _expiredCount = 0;
            _activeCount = 0;
            _expiryCache.Clear();

            var bookings = await _service.GetBookingsByRoomAsync(Room.Id);

            Bookings.Clear();
            foreach (var b in bookings)
                Bookings.Add(b);

            Initialize();
            UpdateCounter();
        }
        catch
        {
            // gestione errori
        }
    }

    private void Initialize()
    {
        foreach (var b in Bookings)
        {
            bool expired = b.IsExpired();

            _expiryCache[b.Id] = expired;

            if (expired)
                _expiredCount++;
            else
                _activeCount++;
        }
    }

    public void Dispose()
    {
        _timeService.Tick -= TimeService_Tick;
        _expiryCache.Clear();
    }
}