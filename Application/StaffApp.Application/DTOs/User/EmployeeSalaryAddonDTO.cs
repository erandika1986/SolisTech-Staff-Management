using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeSalaryAddonDTO
    {
        public int Id { get; set; }
        public string SalaryAddon { get; set; }
        public int SalaryAddonId { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal AdjustedValue { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public bool ApplyForAllEmployees { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public SalaryAddonType SalaryAddonType { get; set; }
        public bool ConsiderForSocialSecurityScheme { get; set; }

        public bool IsPayeApplicable { get; set; }
    }
}
