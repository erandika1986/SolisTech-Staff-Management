using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class CompanyYear : BaseEntity
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveBalance>();
    }
}
