using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum ProjectStatus
    {
        [Description("Not Started")]
        NotStarted = 0,
        [Description("In Progress")]
        InProgress = 1,
        [Description("Completed")]
        Completed = 2,
        [Description("On Hold")]
        OnHold = 3,
        [Description("Cancelled")]
        Cancelled = 4
    }
}
