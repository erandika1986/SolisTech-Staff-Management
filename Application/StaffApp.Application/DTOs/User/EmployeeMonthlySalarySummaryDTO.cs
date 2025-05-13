namespace StaffApp.Application.DTOs.User
{
    public class EmployeeMonthlySalarySummaryDTO
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string BasicSalary { get; set; }
        public string TotalEarning { get; set; }
        public string EmployerContribution { get; set; }
        public string TotalDeduction { get; set; }
        public string NetSalary { get; set; }
        public bool IsRevised { get; set; }
    }
}
