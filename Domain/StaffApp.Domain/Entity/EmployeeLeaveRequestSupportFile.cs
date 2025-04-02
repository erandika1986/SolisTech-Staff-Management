using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeLeaveRequestSupportFile : BaseEntity
    {
        public int EmployeeLeaveRequestId { get; set; }
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }
        public string SaveFileURL { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeLeaveRequest EmployeeLeaveRequest { get; set; }
    }
}
