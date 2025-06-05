using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum TimeCardStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Submitted")]
        Submitted = 2,
        [Description("FullyApproved")]
        FullyApproved = 3,
        [Description("Partially Approved")]
        PartiallyApproved = 4,
        [Description("Partially Rejected")]
        PartiallyRejected = 5,
        [Description("Partially Approved And Rejected")]
        PartiallyApprovedAndRejected = 6,
        [Description("Fully Rejected")]
        FullyRejected = 7,
        [Description("On Leave")]
        OnLeave = 8,
    }
    public enum TimeCardEntryStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Submitted")]
        Submitted = 2,
        [Description("Approved")]
        Approved = 3,
        [Description("Rejected")]
        Rejected = 4
    }
}
