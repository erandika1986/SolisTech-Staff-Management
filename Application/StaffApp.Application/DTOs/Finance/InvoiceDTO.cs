using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Finance
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Invoice Date")]
        public string InvoiceDate { get; set; }

        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public List<InvoiceDetailDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailDTO>();
    }

    public class InvoiceDetailDTO
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string? EmployeeId { get; set; }
        public string Description { get; set; }
        public decimal TotalHours { get; set; }
        public decimal Amount { get; set; }
    }
}
