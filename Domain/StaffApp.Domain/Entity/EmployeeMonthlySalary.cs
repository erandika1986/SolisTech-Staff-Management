using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeMonthlySalary : BaseAuditableEntity
    {
        public int MonthlySalaryId { get; set; }
        public int EmployeeSalaryId { get; set; }
        //public int CompanyYearId { get; set; }
        //public Month Month { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TotalEarning { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsRevised { get; set; }
        public EmployeeSalaryStatus Status { get; set; }

        public virtual EmployeeSalary EmployeeSalary { get; set; }
        public virtual MonthlySalary MonthlySalary { get; set; }

        public virtual ICollection<EmployeeMonthlySalaryAddon> EmployeeMonthlySalaryAddons { get; set; } = new HashSet<EmployeeMonthlySalaryAddon>();
    }
}
