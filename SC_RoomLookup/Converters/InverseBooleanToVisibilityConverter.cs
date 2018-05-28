using System.Windows;

namespace SC_RoomLookup
{
    public sealed class InverseBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InverseBooleanToVisibilityConverter() :
            base(Visibility.Hidden, Visibility.Visible)
        { }
    }
}
