using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeDepartment : BaseEntity
    {
        public string UserId { get; set; }
        public int DepartmentId { get; set; }

        //public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Department Department { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
