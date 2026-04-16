using BookingRoom.Client.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookingRoom.Client.WPF.Converter;

public class StateToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            BookingState.Past => 0.5,
            BookingState.Disabled => 0.4,
            _ => 1.0
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class DensityToPaddingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Density.Compact => new System.Windows.Thickness(6),
            Density.Normal => new System.Windows.Thickness(10),
            Density.Comfortable => new System.Windows.Thickness(14),
            _ => new System.Windows.Thickness(10)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
