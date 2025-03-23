using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; }
        public int DefaultDays { get; set; }
        public bool HasLeaveTypeLogic { get; set; }

        public virtual ICollection<EmployeeLeaveRequest> EmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();
        public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveBalance>();
        public virtual ICollection<LeaveTypeLogic> LeaveTypeLogics { get; set; } = new HashSet<LeaveTypeLogic>();
    }
}
