using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Master
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string DepartmentHeadId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public virtual ApplicationUser DepartmentHead { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
        public virtual ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new HashSet<EmployeeDepartment>();
    }
}
