using System.ComponentModel;

namespace SC_RoomLookup
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Hall
    {
        [Description("Arlington Hall")]
        A_Hall,
        [Description("KC Hall")]
        KC_Hall,
        [Description("Libscomb Hall")]
        Libscomb_Hall,
        [Description("Vandergriff Hall")]
        Vandergriff_Hall
    }

}
