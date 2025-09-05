using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class UserAppraisalCriteria : BaseEntity
    {
        public string CriteriaName { get; set; }
        public string Description { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserAppraisalDetail> UserAppraisalDetails { get; set; } = new HashSet<UserAppraisalDetail>();
    }
}
