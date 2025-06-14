using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.TimeCard
{
    public class MonthlyTimeCardDTO
    {
        [Display(Name = "Time Card Id")]
        public int TimeCardId { get; set; }

        [Display(Name = "Company Year")]
        public int CompanyYear { get; set; }

        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "Day")]
        public int Day { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Generated On")]
        public string GeneratedOn { get; set; }

        [Display(Name = "Status Name")]
        public string StatusName { get; set; }

        public TimeCardStatus Status { get; set; }
    }
}
