using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookingRoom.Client.WPF.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IBookingService _service;
    private readonly IThemeService _themeService;
    private readonly ITimeService _timeService;

    [ObservableProperty]
    private ObservableCollection<RoomViewModel> _rooms = [];

    public DayBookingsContainerViewModel DayBookingsContainer { get; }

    public MainViewModel(
        IBookingService service,
        ITimeService timeService,
        IThemeService themeService)
    {
        _service = service;
        _timeService = timeService;
        _themeService = themeService;
        DayBookingsContainer = new DayBookingsContainerViewModel(_service, _timeService, _themeService);
    }

    [RelayCommand]
    public async Task LoadRoomsAsync()
    {
        try
        {
            var rooms = await _service.GetRoomsAsync();

            Rooms.Clear();

            foreach (var room in rooms)
                Rooms.Add(new RoomViewModel(room, _service, _timeService, _themeService));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}