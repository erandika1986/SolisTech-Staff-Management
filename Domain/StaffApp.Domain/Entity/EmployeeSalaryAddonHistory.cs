using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeSalaryAddonHistory : BaseAuditableEntity
    {
        public int EmployeeSalaryHistoryId { get; set; }
        public int EmployeeSalaryId { get; set; }
        public int SalaryAddonId { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal AdjustedValue { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public bool ConsiderForSocialSecurityScheme { get; set; }

        public virtual EmployeeSalaryHistory EmployeeSalaryHistory { get; set; }
        public virtual EmployeeSalary EmployeeSalary { get; set; }
        public virtual SalaryAddon SalaryAddon { get; set; }
    }
}
