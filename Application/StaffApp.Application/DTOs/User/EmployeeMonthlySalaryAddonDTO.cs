namespace StaffApp.Application.DTOs.User
{
    public class EmployeeMonthlySalaryAddonDTO
    {
        public int Id { get; set; }
        public int EmployeeMonthlySalaryId { get; set; }
        public string SalaryAddon { get; set; }
        public int SalaryAddonId { get; set; }
        public decimal Amount { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal AdjustedValue { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public bool ApplyForAllEmployees { get; set; }

        public bool IsPayeApplicable { get; set; }
    }
}
