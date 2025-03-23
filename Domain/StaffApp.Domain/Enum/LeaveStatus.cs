using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum LeaveStatus
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Approved")]
        Approved = 2,

        [Description("Rejected")]
        Rejected = 3
    }
}
