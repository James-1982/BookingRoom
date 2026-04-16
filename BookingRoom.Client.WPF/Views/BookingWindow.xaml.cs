using BookingRoom.Client.WPF.ViewModels;
using BookingRoom.DTO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BookingRoom.Client.WPF.Views;

public partial class BookingWindow : Window
{
    public BookingDto? Result { get; private set; }

    public BookingWindow(BookingWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not BookingWindowViewModel vm)
            return;

        Result = vm.BuildBooking();

        DialogResult = true;
        Close();
    }
}
