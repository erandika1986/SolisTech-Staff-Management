using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeSalary : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public decimal BaseSalary { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public EmployeeSalaryStatus Status { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<EmployeeSalaryAddon> EmployeeSalaryAddons { get; set; } = new HashSet<EmployeeSalaryAddon>();
        public virtual ICollection<EmployeeMonthlySalary> EmployeeMonthlySalaries { get; set; } = new HashSet<EmployeeMonthlySalary>();
        public virtual ICollection<EmployeeSalaryHistory> EmployeeSalaryHistories { get; set; } = new HashSet<EmployeeSalaryHistory>();
        public virtual ICollection<EmployeeSalaryAddonHistory> EmployeeSalaryAddonHistories { get; set; } = new HashSet<EmployeeSalaryAddonHistory>();
        public virtual ICollection<EmployeeSalaryComment> EmployeeSalaryComments { get; set; } = new HashSet<EmployeeSalaryComment>();

    }
}
