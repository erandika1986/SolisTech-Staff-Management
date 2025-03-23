using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeSalary : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public decimal Salary { get; set; }
        public bool IsIncrement { get; set; }
        public decimal IncrementAmount { get; set; }
        public string? IncrementReason { get; set; }
        public decimal? IncrementPercentage { get; set; }

        public virtual ApplicationUser User { get; set; }


    }
}
