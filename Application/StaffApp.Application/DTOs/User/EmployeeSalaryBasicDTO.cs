using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.User
{
    public class EmployeeSalaryBasicDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Basic Salary")]
        public string BasicSalary { get; set; }

        [Display(Name = "Effective From")]
        public string EffectiveFrom { get; set; }

        [Display(Name = "Status")]
        public string StatusText { get; set; }
        public EmployeeSalaryStatus Status { get; set; }

        [Display(Name = "Last Updated On")]
        public string UpdatedOn { get; set; }

        [Display(Name = "Last Updated By")]
        public string UpdatedBy { get; set; }
    }
}
