using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Appraisal
{
    public class AppraisalPeriodDTO
    {
        public int Id { get; set; }

        [Display(Name = "Appraisal Period Name")]
        public string AppraisalPeriodName { get; set; }

        [Display(Name = "Company Year")]
        public int CompanyYearId { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Display(Name = "Appraisal Status")]
        public string AppraisalStatus { get; set; }
    }
}
