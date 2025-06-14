using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class ExpenseType : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; } = new HashSet<Expense>();
    }
}
