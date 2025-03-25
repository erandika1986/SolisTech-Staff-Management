using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class EmployeeType : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<LeaveTypeConfig> LeaveTypeConfigurations { get; set; } = new HashSet<LeaveTypeConfig>();
        public virtual ICollection<ApplicationUser> Employees { get; set; } = new HashSet<ApplicationUser>();
    }
}
