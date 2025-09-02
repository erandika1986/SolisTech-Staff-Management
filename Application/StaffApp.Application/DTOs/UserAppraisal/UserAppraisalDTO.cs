using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.UserAppraisal
{
    public class UserAppraisalDTO
    {
        public int Id { get; set; }
        public int AppraisalPeriodId { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string ReviewerId { get; set; }
        public string ReviewerFullName { get; set; }
        public decimal OverallRating { get; set; }
        public string Comments { get; set; }
        public AppraisalStatus Status { get; set; }
        public List<UserAppraisalDetailDTO> UserAppraisalDetails { get; set; } = new List<UserAppraisalDetailDTO>();
    }

    public class UserAppraisalDetailDTO
    {
        public int Id { get; set; }
        public int AppraisalID { get; set; }
        public int CriteriaID { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
    }
}
