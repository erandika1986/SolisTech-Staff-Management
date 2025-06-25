using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class InvoiceDetail : BaseAuditableEntity
    {
        public int InvoiceId { get; set; }
        public string? EmployeeId { get; set; }
        public string Description { get; set; }

        public decimal? TotalHours { get; set; }
        public decimal Amount { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual ApplicationUser Employee { get; set; }
    }
}
