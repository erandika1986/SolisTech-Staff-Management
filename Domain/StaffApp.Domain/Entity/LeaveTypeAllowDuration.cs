using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class LeaveTypeAllowDuration : BaseEntity
    {
        public int LeaveTypeId { get; set; }
        public LeaveDuration LeaveDuration { get; set; }

        public virtual LeaveType LeaveType { get; set; }
    }
}
