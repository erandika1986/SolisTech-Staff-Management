using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.Appraisal
{
    public class EmployeeAppraisalDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Position { get; set; } = "";
        public string AppraisalPeriod { get; set; }
        public string ManagerComments { get; set; } = "";
        public string DevelopmentAreas { get; set; } = "";
        public string Goals { get; set; } = "";
        public AppraisalStatus Status { get; set; }
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
