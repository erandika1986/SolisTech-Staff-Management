using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Domain.Entity
{
    public class UserQualificationDocument : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public string OriginalFileName { get; set; }
        public string? OtherName { get; set; }
        public string SaveFileName { get; set; }
        public string Path { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public int DocumentNameId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual DocumentName DocumentName { get; set; }
    }
}
