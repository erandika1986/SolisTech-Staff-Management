using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Salary
{
    public class EmployeeSalaryComment : BaseAuditableEntity
    {
        public int EmployeeSalaryId { get; set; }
        public string Comment { get; set; }

        public virtual EmployeeSalary EmployeeSalary { get; set; }
    }
}
