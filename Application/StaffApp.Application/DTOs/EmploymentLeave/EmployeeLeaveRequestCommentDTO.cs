using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class EmployeeLeaveRequestCommentDTO
    {
        public int Id { get; set; }
        public int EmployeeLeaveRequestId { get; set; }
        public LeaveStatus Status { get; set; }
        public string Comment { get; set; }
    }
}
