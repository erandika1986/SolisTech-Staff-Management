using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class MonthlySalary : BaseAuditableEntity
    {
        public int CompanyYearId { get; set; }
        public Month Month { get; set; }
        public EmployeeSalaryTransferStatus Status { get; set; }

        public virtual CompanyYear CompanyYear { get; set; }

        public virtual ICollection<MonthlySalaryComment> MonthlySalaryComments { get; set; } = new HashSet<MonthlySalaryComment>();
        public virtual ICollection<EmployeeMonthlySalary> EmployeeMonthlySalaries { get; set; } = new HashSet<EmployeeMonthlySalary>();
    }
}
