using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeMonthlySalaryAddon : BaseAuditableEntity
    {
        public int EmployeeMonthlySalaryId { get; set; }
        public int SalaryAddonId { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal AdjustedValue { get; set; }
        public decimal Amount { get; set; }
        public bool IsPayeApplicable { get; set; }
        public bool ConsiderForSocialSecurityScheme { get; set; }
        public virtual EmployeeMonthlySalary EmployeeMonthlySalary { get; set; }
        public virtual SalaryAddon SalaryAddon { get; set; }
    }
}
