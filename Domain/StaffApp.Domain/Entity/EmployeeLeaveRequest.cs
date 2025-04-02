using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveRequest : BaseEntity
    {
        public string EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int CompanyYearId { get; set; }
        public string AssignReportingManagerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public decimal NumberOfDays { get; set; }
        public LeaveStatus CurrentStatus { get; set; }
        public LeaveDuration LeaveDuration { get; set; }
        public HalfDaySessionType? HalfDaySessionType { get; set; }
        public ShortLeaveSessionType? ShortLeaveSessionType { get; set; }
        public string? Reason { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUser Employee { get; set; }
        public virtual ApplicationUser AssignReportingManager { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public virtual CompanyYear CompanyYear { get; set; }

        public virtual ICollection<EmployeeLeaveRequestComment> EmployeeLeaveRequestComments { get; set; } = new HashSet<EmployeeLeaveRequestComment>();
        public virtual ICollection<EmployeeLeaveRequestSupportFile> EmployeeLeaveRequestSupportFiles { get; set; } = new HashSet<EmployeeLeaveRequestSupportFile>();
    }
}
