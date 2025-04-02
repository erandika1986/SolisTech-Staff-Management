using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum ShortLeaveSessionType
    {
        [Description("Morning 8.30 AM - 9.30 AM")]
        Morning = 1,
        [Description("Morning 3.30 PM - 4.30 PM")]
        Afternoon = 2
    }
}
