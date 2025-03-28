using StaffApp.Application.DTOs.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class EmployeeLeaveRequestDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public DropDownDTO SelectedLeaveType { get; set; }
        //public int LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal NumberOfDays { get; set; }
        public LeaveStatus CurrentStatus { get; set; }
        public LeaveDuration LeaveDuration { get; set; }
        public string? Reason { get; set; }
        public List<EmployeeLeaveRequestCommentDTO> Comments { get; set; } = new List<EmployeeLeaveRequestCommentDTO>();
    }
}
