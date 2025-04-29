using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class PayeLogic : BaseEntity
    {
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public decimal TaxRate { get; set; }
    }
}
