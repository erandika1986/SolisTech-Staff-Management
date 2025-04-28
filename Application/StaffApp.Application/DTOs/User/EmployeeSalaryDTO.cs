using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeSalaryDTO
    {
        public EmployeeSalaryDTO()
        {
            EmployeeSalaryAddons = new List<EmployeeSalaryAddonDTO>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }

        [Range(35000, 1000000000, ErrorMessage = "Salary should be between greater than or equal to 35000")]
        public decimal BasicSalary { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public EmployeeSalaryStatus Status { get; set; }
        public string StatusString { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }

        public List<EmployeeSalaryAddonDTO> EmployeeSalaryAddons { get; set; }
    }
}
