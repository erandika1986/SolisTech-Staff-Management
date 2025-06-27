using System.ComponentModel;

namespace StaffApp.Domain.Enum
{
    public enum InvoiceStatus
    {
        [Description("Draft")]
        Draft = 1,
        [Description("Submitted for Approval")]
        SubmittedForApproval = 2,
        [Description("Approved")]
        Approved = 3,
        [Description("Sent to Client")]
        Sent = 4,
        [Description("Partially Paid")]
        PartiallyPaid = 5,
        [Description("Paid")]
        Paid = 6,
        [Description("Overdue")]
        Overdue = 7,
        [Description("Rejected")]
        Rejected = 8,
        [Description("Cancelled")]
        Cancelled = 9,
        [Description("Archived")]
        Archived = 10
    }
}
