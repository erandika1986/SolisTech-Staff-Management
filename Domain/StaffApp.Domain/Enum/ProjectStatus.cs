using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum ProjectStatus
    {
        [Description("Not Started")]
        NotStarted = 1,
        [Description("In Progress")]
        InProgress = 2,
        [Description("Completed")]
        Completed = 3,
        [Description("On Hold")]
        OnHold = 4,
        [Description("Cancelled")]
        Cancelled = 5
    }
}
