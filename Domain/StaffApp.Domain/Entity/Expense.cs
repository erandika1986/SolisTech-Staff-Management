using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class Expense : BaseAuditableEntity
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal? CompanyShare { get; set; }
        public decimal? EmployeeShare { get; set; }
        public string Notes { get; set; }
        public int ExpenseTypeId { get; set; }
        public int CompanyYearId { get; set; }
        public Month Month { get; set; }


        public virtual ExpenseType ExpenseType { get; set; }
        public virtual CompanyYear CompanyYear { get; set; }

        public virtual ICollection<ExpenseSupportAttachment> ExpenseSupportAttachments { get; set; } = new HashSet<ExpenseSupportAttachment>();
    }
}
