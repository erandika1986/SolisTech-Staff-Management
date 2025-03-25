using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
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
