using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class SupportAttachment : BaseEntity
    {
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }
        public string SaveFileURL { get; set; }

        public virtual ICollection<ExpenseSupportAttachment> ExpenseSupportAttachments { get; set; } = new HashSet<ExpenseSupportAttachment>();
        public virtual ICollection<IncomeSupportAttachment> IncomeSupportAttachments { get; set; } = new HashSet<IncomeSupportAttachment>();
    }
}
