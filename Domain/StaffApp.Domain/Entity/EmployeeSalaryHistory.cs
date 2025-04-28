using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeSalaryHistory : BaseAuditableEntity
    {
        public int EmployeeSalaryId { get; set; }
        public string UserId { get; set; }
        public decimal BaseSalary { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public EmployeeSalaryStatus? Status { get; set; }

        public virtual EmployeeSalary EmployeeSalary { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<EmployeeSalaryAddonHistory> EmployeeSalaryAddonHistories { get; set; } = new HashSet<EmployeeSalaryAddonHistory>();
    }
}
