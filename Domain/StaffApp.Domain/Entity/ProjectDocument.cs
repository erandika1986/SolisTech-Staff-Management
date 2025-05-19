using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class ProjectDocument : BaseAuditableEntity
    {
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }
        public string SavedPath { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
