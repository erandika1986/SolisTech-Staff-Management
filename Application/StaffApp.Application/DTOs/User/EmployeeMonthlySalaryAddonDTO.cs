using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeMonthlySalaryAddonDTO
    {
        public int Id { get; set; }
        public int EmployeeMonthlySalaryId { get; set; }
        public string SalaryAddon { get; set; }
        public int SalaryAddonId { get; set; }
        public decimal Amount { get; set; }

        private decimal originalValue;
        public decimal OriginalValue
        {
            get { return originalValue; }
            set
            {
                originalValue = value;
            }
        }

        private decimal adjustedValue;
        public decimal AdjustedValue
        {
            get { return adjustedValue; }
            set
            {
                adjustedValue = value;
            }
        }

        public bool ApplyForAllEmployees { get; set; }
        public ProportionType ProportionType { get; set; }
        public SalaryAddonType AddonType { get; set; }

        public bool IsApplicableForPaye { get; set; }
    }
}
