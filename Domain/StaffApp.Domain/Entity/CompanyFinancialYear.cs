using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class CompanyFinancialYear : BaseEntity
    {
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
