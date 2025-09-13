using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Domain.Entity.Leave
{
    public class EmployeeLeaveAllocation : BaseEntity
    {
        public string EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int CompanyYearId { get; set; }
        public decimal AllocatedLeaveCount { get; set; }
        public decimal RemainingLeaveCount { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }


        public virtual ApplicationUser Employee { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public virtual CompanyYear CompanyYear { get; set; }

    }
}
