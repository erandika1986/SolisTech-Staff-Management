using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeMonthlySalaryDTO
    {
        public int Id { get; set; }
        public int EmployeeSalaryId { get; set; }
        public int MonthlySalaryId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TotalEarning { get; set; }
        public decimal EmployerContribution { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedByUserId { get; set; }
        public bool IsActive { get; set; }

        public EmployeeSalaryTransferStatus Status { get; set; }

        public List<EmployeeMonthlySalaryAddonDTO> EmployeeSalaryAddons { get; set; } = new List<EmployeeMonthlySalaryAddonDTO>();
    }

}
