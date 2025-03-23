using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveRequest : BaseAuditableEntity
    {
        public string EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public virtual ApplicationUser Employee { get; set; }
        public virtual LeaveType LeaveType { get; set; }

        public virtual ICollection<EmployeeLeaveApproval> EmployeeLeaveApprovals { get; set; } = new HashSet<EmployeeLeaveApproval>();
    }
}
