using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class ProjectMember : BaseAuditableEntity
    {
        public int ProjectId { get; set; }
        public string MemberId { get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser Member { get; set; }
        //public virtual ApplicationRole Role { get; set; }
    }
}
