using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeMonthlySalary : BaseAuditableEntity
    {
        public int EmployeeSalaryId { get; set; }
        public int CompanyYearId { get; set; }
        public Month Month { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalForPF { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }

        public virtual EmployeeSalary EmployeeSalary { get; set; }
        public virtual CompanyYear CompnayYear { get; set; }

        public virtual ICollection<EmployeeMonthlySalaryAddon> EmployeeMonthlySalaryAddons { get; set; } = new HashSet<EmployeeMonthlySalaryAddon>();
    }
}
