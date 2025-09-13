using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Domain.Entity.Leave
{
    public class LeaveTypeConfig : BaseEntity
    {
        public int EmployeeTypeId { get; set; }
        public int LeaveTypeId { get; set; }

        public decimal AnnualLeaveAllowance { get; set; }
        public int? MinimumServiceMonthsRequired { get; set; }

        public virtual EmployeeType EmployeeType { get; set; }
        public virtual LeaveType LeaveType { get; set; }

    }
}
