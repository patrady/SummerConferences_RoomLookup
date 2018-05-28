using System.Windows;

namespace SC_RoomLookup
{
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Hidden)
        { }
    }
}
