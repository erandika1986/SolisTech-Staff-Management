using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveRequestComment : BaseAuditableEntity
    {
        public int EmployeeLeaveRequestId { get; set; }
        public LeaveStatus Status { get; set; }
        public string Comment { get; set; }

        public virtual EmployeeLeaveRequest EmployeeLeaveRequest { get; set; }
    }
}
