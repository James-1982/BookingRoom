using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using BookingRoom.Client.WPF.Utils;
using BookingRoom.Client.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BookingRoom.Client.WPF.ViewModels;

public partial class DayBookingsContainerViewModel(
    IBookingService service,
    ITimeService timeService,
    IThemeService themeService) : ObservableObject
{
    private readonly IBookingService _service = service;
    private readonly ITimeService _timeService = timeService;
    private readonly IThemeService _themeService = themeService;

    public ObservableCollection<DayBookingsPanelViewModel> Days { get; } = [];

    [ObservableProperty]
    private RoomViewModel? selectedRoom;

    [ObservableProperty]
    private bool isLoading;

    public bool HasActiveBooking =>
    Days?.Any(b => b.Bookings.Any(b => b.State == BookingState.Active)) == true;

    partial void OnSelectedRoomChanged(RoomViewModel? value)
    {
        if (value == null)
            return;

        PlayRoomChangeAnimation(); // ✔ qui è corretto
        _ = LoadBookingsAnimatedAsync();
    }

    public void PlayRoomChangeAnimation()
    {
        AnimationRequested?.Invoke();
    }

    public event Action? AnimationRequested;

    private async Task LoadBookingsAnimatedAsync()
    {
        if (SelectedRoom == null)
        {
            Days.Clear();
            return;
        }

        IsLoading = true;

        await Task.Delay(120); // permette fade-out UI

        var bookings = await _service.GetBookingsByRoomAsync(SelectedRoom.Room.Id);

        Days.Clear();

        var grouped = bookings
            .GroupBy(b => b.StartTime.Date)
            .OrderBy(g => g.Key);

        foreach (var group in grouped)
        {
            Days.Add(new DayBookingsPanelViewModel(
                SelectedRoom.Theme,
                group,
                _timeService,
                _themeService));
        }

        IsLoading = false;

        OnPropertyChanged(nameof(HasActiveBooking));
    }

    [RelayCommand]
    private async Task CreateBookingAsync()
    {
        var vm = new BookingWindowViewModel(SelectedRoom);

        var window = new BookingWindow(vm);

        if (window.ShowDialog() == true)
        {
            var booking = window.Result;
            if (await _service.CreateBookingAsync(booking))
            {
                await LoadBookingsAnimatedAsync();

                if (SelectedRoom != null)
                    await SelectedRoom.LoadBookingsCommand.ExecuteAsync(null);
            }
        }
    }
}