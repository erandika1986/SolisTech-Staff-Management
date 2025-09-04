using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class UserDisciplinaryAction : BaseAuditableEntity
    {

        // Foreign key to your User table
        public string UserId { get; set; }
        public DisciplinaryActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? EffectiveUntil { get; set; }
        public string Reason { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public string? Remarks { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
