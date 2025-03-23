using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveBalance : BaseAuditableEntity
    {
        public string EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int CompanyYearId { get; set; }
        public decimal AllocatedLeaveCount { get; set; }
        public decimal RemainingLeaveCount { get; set; }


        public virtual ApplicationUser Employee { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public virtual CompanyYear CompanyYear { get; set; }

    }
}
