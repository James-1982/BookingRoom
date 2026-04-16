using BookingRoom.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BookingRoom.Client.WPF.ViewModels;

public partial class BookingWindowViewModel : ObservableObject
{
    private readonly int _roomId;

    public BookingWindowViewModel(RoomViewModel room)
    {
        _roomId = room.Room.Id;
        RoomName = room.Room.Name;

        InitTimeSelectors();
    }

    [ObservableProperty]
    private string roomName = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private int startHour;

    [ObservableProperty]
    private int startMinute;

    [ObservableProperty]
    private int endHour;

    [ObservableProperty]
    private int endMinute;

    public ObservableCollection<int> Hours { get; } = new();
    public ObservableCollection<int> Minutes { get; } = new();

    private void InitTimeSelectors()
    {
        for (int i = 0; i < 24; i++)
            Hours.Add(i);

        for (int i = 0; i < 60; i += 5)
            Minutes.Add(i);
    }

    public BookingDto BuildBooking()
    {
        var start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartHour, StartMinute, 0);
        var end = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndHour, EndMinute, 0);

        return new BookingDto
        {
            Id = Random.Shared.Next(1000, 9999),
            RoomId = _roomId,
            Title = Title,
            StartTime = start,
            EndTime = end
        };
    }

    public bool CanConfirm =>
        !string.IsNullOrWhiteSpace(Title)
        && StartDate != default
        && EndDate != default
        && IsValidTimeRange();

    partial void OnStartDateChanged(DateTime value) => OnPropertyChanged(nameof(CanConfirm));
    partial void OnEndDateChanged(DateTime value) => OnPropertyChanged(nameof(CanConfirm));

    partial void OnStartHourChanged(int value) => OnPropertyChanged(nameof(CanConfirm));
    partial void OnStartMinuteChanged(int value) => OnPropertyChanged(nameof(CanConfirm));

    partial void OnEndHourChanged(int value) => OnPropertyChanged(nameof(CanConfirm));
    partial void OnEndMinuteChanged(int value) => OnPropertyChanged(nameof(CanConfirm));

    partial void OnTitleChanged(string value) => OnPropertyChanged(nameof(CanConfirm));

    private bool IsValidTimeRange()
    {
        var start = new DateTime(
            StartDate.Year, StartDate.Month, StartDate.Day,
            StartHour, StartMinute, 0);

        var end = new DateTime(
            EndDate.Year, EndDate.Month, EndDate.Day,
            EndHour, EndMinute, 0);

        return end > start;
    }

    //public bool IsValid()
    //    => !string.IsNullOrWhiteSpace(Title)
    //       && EndDate >= StartDate;
}