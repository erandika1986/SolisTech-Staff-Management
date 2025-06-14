using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.TimeCard
{
    public class BasicTimeCardDTO
    {
        [Display(Name = "Time Card Id")]
        public int Id { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }


        public DateTime Date { get; set; }

        [Display(Name = "Date")]
        public string DateByString { get; set; }

        public TimeCardStatus Status { get; set; }
        [Display(Name = "Status")]
        public string StatusName { get; set; }


        [Display(Name = "Number Of Projects")]
        public int NumberOfProjects { get; set; }


        [Display(Name = "Total Hours")]
        public double TotalHours { get; set; }
    }

    public class BasicManagerTimeCardDTO
    {
        [Display(Name = "Time Card Id")]
        public int TimeCardId { get; set; }


        [Display(Name = "Time Card Entry Id")]
        public int TimeCardEntryId { get; set; }


        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Date")]
        public string DateByString { get; set; }


        [Display(Name = "Status")]
        public string StatusName { get; set; }
        public TimeCardEntryStatus Status { get; set; }


        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }


        [Display(Name = "Total Hours")]
        public double TotalHours { get; set; }
    }
}
