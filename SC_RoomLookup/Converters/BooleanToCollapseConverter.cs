using System.Windows;

namespace SC_RoomLookup
{
    public sealed class BooleanToCollapseConverter : BooleanConverter<Visibility>
    {
        public BooleanToCollapseConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
