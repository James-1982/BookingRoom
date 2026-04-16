using BookingRoom.Client.WPF.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookingRoom.Client.WPF.Views;

public partial class BookingCardView : UserControl
{
    public BookingCardView()
    {
        InitializeComponent();
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        if (DataContext is BookingViewModel vm)
            vm.IsMouseOver = true;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        if (DataContext is BookingViewModel vm)
            vm.IsMouseOver = false;
    }
}