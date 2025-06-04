namespace StaffApp.Application.DTOs.TimeCard
{
    public class BasicTimeCardDTO
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public string StatusName { get; set; }
        public int NumberOfProjects { get; set; }
        public double TotalHours { get; set; }
    }
}
