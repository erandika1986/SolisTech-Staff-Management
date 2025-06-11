namespace StaffApp.Application.DTOs.TimeCard
{
    public class MonthlyTimeCardDTO
    {
        public int TimeCardId { get; set; }
        public int CompanyYear { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string EmployeeName { get; set; }
        public string GeneratedOn { get; set; }
    }
}
