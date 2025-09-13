using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Master;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class Invoice : BaseAuditableEntity
    {
        public int CompanyYearId { get; set; }
        public Month Month { get; set; }
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime PeriodStart { get; set; }

        public DateTime PeriodEnd { get; set; }

        public int? ProjectId { get; set; }
        public decimal? TotalHours { get; set; }

        public decimal TotalAmount { get; set; }

        public string Notes { get; set; }
        public InvoiceStatus Status { get; set; }


        public virtual CompanyYear CompanyYear { get; set; }
        public virtual Project Project { get; set; }

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new HashSet<InvoiceDetail>();
        public virtual ICollection<InvoicePayment> InvoicePayments { get; set; } = new HashSet<InvoicePayment>();
    }
}
