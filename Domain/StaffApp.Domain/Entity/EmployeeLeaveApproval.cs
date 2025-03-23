using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveApproval : BaseAuditableEntity
    {
        public int EmployeeLeaveRequestId { get; set; }
        public LeaveStatus Status { get; set; }
        public string? Comments { get; set; }

        public virtual EmployeeLeaveRequest EmployeeLeaveRequest { get; set; }
    }
}
