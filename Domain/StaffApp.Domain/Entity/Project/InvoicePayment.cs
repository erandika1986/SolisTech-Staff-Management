using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class InvoicePayment : BaseAuditableEntity
    {
        public int InvoiceId { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
