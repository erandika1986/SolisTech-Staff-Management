using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class Project : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectManagementPlatform ManagementPlatform { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ManagerId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public ProjectStatus Status { get; set; }

        public virtual ApplicationUser Manager { get; set; }

        public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new HashSet<ProjectMember>();
        public virtual ICollection<ProjectDocument> ProjectDocuments { get; set; } = new HashSet<ProjectDocument>();
        public virtual ICollection<TimeCardEntry> TimeCardEntries { get; set; } = new HashSet<TimeCardEntry>();
    }
}
