using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum AppraisalStatus
    {
        [Description("Pending")]
        Pending = 1,

        [Description("In Review")]
        InReview = 2,

        [Description("Completed")]
        Completed = 3,
    }
}
