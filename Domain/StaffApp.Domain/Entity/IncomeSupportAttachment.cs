using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class IncomeSupportAttachment : BaseEntity
    {
        public int IncomeId { get; set; }
        public int SupportAttachmentId { get; set; }

        public virtual Income Income { get; set; }
        public virtual SupportAttachment SupportAttachment { get; set; }
    }
}
