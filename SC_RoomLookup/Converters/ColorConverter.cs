using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SC_RoomLookup
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Hall)value)
            {
                case Hall.A_Hall:
                    return (SolidColorBrush)Application.Current.Resources["AHall"];
                case Hall.KC_Hall:
                    return (SolidColorBrush)Application.Current.Resources["KCHall"];
                case Hall.Libscomb_Hall:
                    return (SolidColorBrush)Application.Current.Resources["LibscombHall"];
                case Hall.Vandergriff_Hall:
                    return (SolidColorBrush)Application.Current.Resources["Vandergriff"];
                default:
                    return (SolidColorBrush)Application.Current.Resources["LightGray1"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
