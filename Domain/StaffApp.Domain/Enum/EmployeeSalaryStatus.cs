using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum EmployeeSalaryStatus
    {
        [Description("Submitted For Approval")]
        SubmittedForApproval = 1,

        [Description("Approved")]
        Approved = 2,

        [Description("Submitted For Revised")]
        SubmittedForRevised = 3
    }
}
