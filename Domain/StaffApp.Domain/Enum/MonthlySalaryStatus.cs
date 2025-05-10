using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum MonthlySalaryStatus
    {
        [Description("Generated")]
        Generated = 1,
        [Description("Submitted For Approval")]
        SubmittedForApproval = 2,
        [Description("Approved")]
        Approved = 3,
        [Description("Submitted For Revised")]
        SubmittedForRevised = 4,
        [Description("Submitted To Bank")]
        SubmittedToBank = 5,
        [Description("Transferred")]
        Transferred = 6,
        [Description("Not Generated")]
        NotGenerated = 7
    }
}
