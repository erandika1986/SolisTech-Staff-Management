using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.UserAppraisal
{
    public class UserAppraisalSummaryDTO
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
    }
}
