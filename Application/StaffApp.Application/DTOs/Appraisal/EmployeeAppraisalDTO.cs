using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Appraisal
{
    public class EmployeeAppraisalDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = "";

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; } = "";

        [Display(Name = "Department")]
        public string Department { get; set; } = "";

        [Display(Name = "Position")]
        public string Position { get; set; } = "";

        [Display(Name = "Appraisal Period")]
        public string AppraisalPeriod { get; set; }
        public string ManagerComments { get; set; } = "";
        public string DevelopmentAreas { get; set; } = "";
        public string Goals { get; set; } = "";
        public AppraisalStatus Status { get; set; }

        [Display(Name = "Reviewer Name")]
        public string? ReviewerName { get; set; }

        [Display(Name = "Reviewed On")]
        public string? ReviewedOn { get; set; }

        [Display(Name = "Overall Rating")]
        public double? OverallRating { get; set; }
        public string? CompanyYear { get; set; }
        public List<AppraisalCriteriaDTO> AppraisalCriteria { get; set; } = new();
    }

    public class AppraisalCriteriaDTO
    {
        public int Id { get; set; }
        public int AppraisalID { get; set; }
        public string CriteriaName { get; set; } = "";
        public int CriteriaId { get; set; }
        public double Rating { get; set; }
        public string Comments { get; set; } = "";
    }
}
