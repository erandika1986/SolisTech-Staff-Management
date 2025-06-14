using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class ExpenseSupportAttachment : BaseEntity
    {
        public int ExpenseId { get; set; }
        public int SupportAttachmentId { get; set; }

        public virtual Expense Expense { get; set; }
        public virtual SupportAttachment SupportAttachment { get; set; }
    }
}
