using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity.Appraisal
{
    public class UserAppraisal : BaseAuditableEntity
    {
        public int AppraisalPeriodId { get; set; }
        public string UserId { get; set; }
        public string? ReviewerId { get; set; }
        public double OverallRating { get; set; }
        public string Comments { get; set; }
        public string AreaForDevelopment { get; set; }
        public string GoalsForNextPeriod { get; set; }
        public AppraisalStatus Status { get; set; }

        public virtual AppraisalPeriod AppraisalPeriod { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Reviewer { get; set; }

        public virtual ICollection<UserAppraisalDetail> UserAppraisalDetails { get; set; } = new HashSet<UserAppraisalDetail>();
    }
}
