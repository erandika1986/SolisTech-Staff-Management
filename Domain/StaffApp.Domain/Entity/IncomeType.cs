using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class IncomeType : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Income> Incomes { get; set; } = new HashSet<Income>();
    }
}
