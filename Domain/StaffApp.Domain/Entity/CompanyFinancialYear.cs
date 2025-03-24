using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class CompanyFinancialYear : BaseEntity
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentYear { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }
    }
}
