using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class SalaryAddon : BaseAuditableEntity
    {
        public string Name { get; set; }
        public SalaryAddonType AddonType { get; set; }
        public ProportionType ProportionType { get; set; }
        public decimal DefaultValue { get; set; }
        public bool ApplyForAllEmployees { get; set; }

        public virtual ICollection<EmployeeSalaryAddon> EmployeeSalaryAddons { get; set; } = new HashSet<EmployeeSalaryAddon>();
        public virtual ICollection<EmployeeMonthlySalaryAddon> EmployeeMonthlySalaryAddons { get; set; } = new HashSet<EmployeeMonthlySalaryAddon>();
        public virtual ICollection<EmployeeSalaryAddonHistory> EmployeeSalaryAddonHistories { get; set; } = new HashSet<EmployeeSalaryAddonHistory>();
    }
}
