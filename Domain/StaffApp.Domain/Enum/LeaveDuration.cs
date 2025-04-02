using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum LeaveDuration
    {
        [Description("Full Day")]
        FullDay = 1,
        [Description("Half Day")]
        HalfDay = 2,
        [Description("Short Leave")]
        ShortLeave = 3,
    }
}
