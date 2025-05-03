using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class MonthlySalaryComment : BaseAuditableEntity
    {
        public int MonthlySalaryId { get; set; }
        public string Comment { get; set; }

        public virtual MonthlySalary MonthlySalary { get; set; }
    }
}
