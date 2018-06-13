using System.ComponentModel;

namespace SC_RoomLookup
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SearchBy_Settings
    {
        [Description("PIK")]
        PIK,
        [Description("Room Number")]
        RoomNumber,
        [Description("Key Code")]
        KeyCode
    }

}
