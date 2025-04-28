namespace StaffApp.Domain.Entity.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

    }
}
