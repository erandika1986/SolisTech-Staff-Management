using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class Expense : BaseAuditableEntity
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public int ExpenseTypeId { get; set; }


        public virtual ExpenseType ExpenseType { get; set; }

        public virtual ICollection<ExpenseSupportAttachment> ExpenseSupportAttachments { get; set; } = new HashSet<ExpenseSupportAttachment>();
    }
}
