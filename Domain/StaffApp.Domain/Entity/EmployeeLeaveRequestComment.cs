using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveRequestComment : BaseEntity
    {
        public int EmployeeLeaveRequestId { get; set; }

        public LeaveStatus Status { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }

        public virtual EmployeeLeaveRequest EmployeeLeaveRequest { get; set; }
    }
}
