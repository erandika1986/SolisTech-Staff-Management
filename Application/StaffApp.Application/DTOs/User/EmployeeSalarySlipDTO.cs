namespace StaffApp.Application.DTOs.User
{
    public class EmployeeSalarySlipDTO
    {
        public string CompanyName { get; set; }
        public string LogoUrl { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        public string SalarySlipYear { get; set; }
        public string SalarySlipMonth { get; set; }
        public string SalarySlipNumber { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeId { get; set; }
        public string Designation { get; set; }
        public string PayDate { get; set; }
        public string PayPeriod { get; set; }
        public string PaymentMethod { get; set; }
        public string JoinDate { get; set; }

        // Earnings
        public List<PaymentDescriptionDTO> Earnings { get; set; } = new List<PaymentDescriptionDTO>();
        public List<PaymentDescriptionDTO> Deductions { get; set; } = new List<PaymentDescriptionDTO>();
        public List<PaymentDescriptionDTO> EmployerContributions { get; set; } = new List<PaymentDescriptionDTO>();

        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
    }

    public class PaymentDescriptionDTO
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
