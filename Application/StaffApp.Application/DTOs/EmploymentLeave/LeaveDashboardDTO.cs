namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class LeaveDashboardDTO
    {
        public LeaveDashboardDTO()
        {
            LeaveTypeSummaries = new List<MyLeaveTypeSummaryDTO>();
        }
        public List<MyLeaveTypeSummaryDTO> LeaveTypeSummaries { get; set; }
    }

    public class MyLeaveTypeSummaryDTO
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public decimal TotalLeaveDays { get; set; }
        public decimal RemainingLeaveDays { get; set; }
        public decimal ApprovedLeaveDays { get; set; }
        public decimal PendingLeaveDays { get; set; }
        public decimal RejectedLeaveDays { get; set; }
    }
}
