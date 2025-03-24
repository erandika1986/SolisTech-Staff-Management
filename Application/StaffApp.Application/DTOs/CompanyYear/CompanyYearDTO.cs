namespace StaffApp.Application.DTOs.CompanyYear
{
    public class CompanyYearDTO
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrentYear { get; set; }
        public string CurrentYear { get; set; }
    }
}
