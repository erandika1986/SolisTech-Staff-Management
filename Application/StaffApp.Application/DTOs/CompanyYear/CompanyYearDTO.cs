using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.CompanyYear
{
    public class CompanyYearDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public int Year { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Is Current Year")]
        public bool IsCurrentYear { get; set; }

        [Display(Name = "Current Year")]
        public string CurrentYear { get; set; }
    }
}
