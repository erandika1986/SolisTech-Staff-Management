using StaffApp.Application.DTOs.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class EmployeeLeaveRequestDTO
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DropDownDTO SelectedCompanyYear { get; set; }
        public UserDropDownDTO SelectedReportingManager { get; set; }
        public DropDownDTO SelectedLeaveType { get; set; }
        public DropDownDTO SelectedLeaveDuration { get; set; }
        public DropDownDTO SelectedHalfDaySessionType { get; set; }
        public DropDownDTO SelectedShortLeaveSessionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal NumberOfDays { get; set; }
        public LeaveStatus CurrentStatus { get; set; }
        public string? Reason { get; set; }
        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
        public List<EmployeeLeaveRequestSupportFileDTO> SavedSupportFiles { get; set; } = new List<EmployeeLeaveRequestSupportFileDTO>();
        public bool CancelLeaveAllows { get; set; }
    }
}
