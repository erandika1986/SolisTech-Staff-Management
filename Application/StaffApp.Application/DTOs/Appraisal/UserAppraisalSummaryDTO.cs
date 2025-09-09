using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Appraisal
{
    public class UserAppraisalSummaryDTO
    {
        public int Id { get; set; }

        [Display(Name = "Appraisal Period")]
        public string AppraisalPeriod { get; set; }

        [Display(Name = "Employee Name")]
        public string UserFullName { get; set; }

        [Display(Name = "Reviewer Name")]
        public string ReviewerName { get; set; }

        [Display(Name = "Overall Rating")]
        public double OverallRating { get; set; }

        [Display(Name = "Comments")]
        public string Comments { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
    }
}
