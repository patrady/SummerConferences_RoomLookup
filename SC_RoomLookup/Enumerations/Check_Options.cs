using System.ComponentModel;

namespace SC_RoomLookup
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Check_Options
    {
        [Description("None")]
        None,
        [Description("Check In")]
        Check_In,
        [Description("Check Out")]
        Check_Out
    }

}
