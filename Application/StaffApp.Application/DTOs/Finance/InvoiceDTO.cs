namespace StaffApp.Application.DTOs.Finance
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public List<InvoiceDetailDTO> InvoiceDetails { get; set; } = new List<InvoiceDetailDTO>();
    }

    public class InvoiceDetailDTO
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
