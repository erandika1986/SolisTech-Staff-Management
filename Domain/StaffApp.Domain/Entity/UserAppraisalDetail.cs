using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class UserAppraisalDetail : BaseEntity
    {
        public int AppraisalID { get; set; }
        public int CriteriaID { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }

        public virtual UserAppraisal UserAppraisal { get; set; }
        public virtual UserAppraisalCriteria UserAppraisalCriteria { get; set; }
    }
}
