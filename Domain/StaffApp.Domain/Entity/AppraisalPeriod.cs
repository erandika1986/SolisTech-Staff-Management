using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class AppraisalPeriod : BaseEntity
    {
        public string AppraisalPeriodName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<UserAppraisal> UserAppraisals { get; set; } = new HashSet<UserAppraisal>();
    }
}
