using System;
using System.Globalization;
using System.Windows.Data;

namespace SC_RoomLookup
{
    public class Last4DigitsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value as string;
            
            if (input.Length >= 4)
                return input.Substring(input.Length - 4, 4);
            return input;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
