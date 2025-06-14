using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class Income : BaseAuditableEntity
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public int IncomeTypeId { get; set; }

        public virtual IncomeType IncomeType { get; set; }

        public virtual ICollection<IncomeSupportAttachment> IncomeSupportAttachments { get; set; } = new HashSet<IncomeSupportAttachment>();
    }
}
