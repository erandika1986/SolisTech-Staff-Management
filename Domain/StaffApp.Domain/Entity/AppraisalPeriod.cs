using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class AppraisalPeriod : BaseEntity
    {
        public string AppraisalPeriodName { get; set; }
        public int CompanyYearId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public AppraisalStatus AppraisalStatus { get; set; }

        public virtual CompanyYear CompanyYear { get; set; }
        public virtual ICollection<UserAppraisal> UserAppraisals { get; set; } = new HashSet<UserAppraisal>();
    }
}
