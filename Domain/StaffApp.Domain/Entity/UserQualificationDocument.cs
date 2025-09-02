using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class UserQualificationDocument : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public string DocumentName { get; set; }
        public string OriginalFileName { get; set; }
        public string SaveFileName { get; set; }
        public string Path { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
