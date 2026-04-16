using BookingRoom.Client.WPF.Services;
using BookingRoom.Client.WPF.Services.Themes;
using BookingRoom.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BookingRoom.Client.WPF.ViewModels;

public class DayBookingsPanelViewModel : ObservableObject
{
    public DateTime Date { get; }

    public ObservableCollection<BookingViewModel> Bookings { get; } = [];

    public DayBookingsPanelViewModel(
        Theme roomTheme,
        IEnumerable<BookingDto> bookings,
        ITimeService timeService,
        IThemeService themeService)
    {
        Date = bookings.FirstOrDefault()?.StartTime.Date ?? DateTime.MinValue;

        Bookings = new ObservableCollection<BookingViewModel>(
            bookings.OrderBy(b => b.StartTime)
                    .Select(b => new BookingViewModel(b, roomTheme, timeService, themeService)));
    }
}