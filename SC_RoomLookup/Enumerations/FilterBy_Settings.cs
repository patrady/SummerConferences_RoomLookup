using System.ComponentModel;

namespace SC_RoomLookup
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FilterBy_Settings
    {
        [Description("Conference Room")]
        Conference,
        [Description("Spare Room")]
        Spare,
        [Description("All")]
        All
    }

}
