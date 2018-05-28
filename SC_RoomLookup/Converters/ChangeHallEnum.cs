using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SC_RoomLookup
{
    public class ChangeHallEnum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Hall)parameter)
            {
                case Hall.A_Hall:
                    return (SolidColorBrush)Application.Current.Resources["FlatRed"];
                case Hall.KC_Hall:
                    return (SolidColorBrush)Application.Current.Resources["FlatPurple"];
                case Hall.Libscomb_Hall:
                    return (SolidColorBrush)Application.Current.Resources["FlatLightBlue"];
                case Hall.Vandergriff_Hall:
                    return (SolidColorBrush)Application.Current.Resources["FlatOrange"];
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
