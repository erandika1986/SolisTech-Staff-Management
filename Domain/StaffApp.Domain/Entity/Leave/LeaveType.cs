using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Leave
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; }
        public int DefaultDays { get; set; }
        public bool HasLeaveTypeLogic { get; set; }
        public int AllowGenderType { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<EmployeeLeaveRequest> EmployeeLeaveRequests { get; set; } = new HashSet<EmployeeLeaveRequest>();
        public virtual ICollection<EmployeeLeaveAllocation> EmployeeLeaveBalances { get; set; } = new HashSet<EmployeeLeaveAllocation>();
        public virtual ICollection<LeaveTypeLogic> LeaveTypeLogics { get; set; } = new HashSet<LeaveTypeLogic>();
        public virtual ICollection<LeaveTypeConfig> LeaveTypeConfigurations { get; set; } = new HashSet<LeaveTypeConfig>();
        public virtual ICollection<LeaveTypeAllowDuration> LeaveTypeAllowDurations { get; set; } = new HashSet<LeaveTypeAllowDuration>();
    }
}
