using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum HalfDaySessionType
    {
        [Description("Morning 8.30 AM - 12.00 PM")]
        Morning = 1,
        [Description("Morning 1.00 PM - 4.30 PM")]
        Afternoon = 2
    }
}
