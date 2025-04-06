using StaffApp.Application.DTOs.Common;
using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class BasicEmployeeLeaveRequestDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Employee Id")]
        public string EmployeeId { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        public DropDownDTO SelectedLeaveType { get; set; }
        //public int LeaveTypeId { get; set; }

        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }

        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Display(Name = "Number Of Days")]
        public decimal NumberOfDays { get; set; }

        [Display(Name = "Current Status")]
        public string CurrentStatus { get; set; }
        public LeaveStatus Status { get; set; }

        [Display(Name = "Leave Duration")]
        public string LeaveDuration { get; set; }

        [Display(Name = "Reason")]
        public string? Reason { get; set; }
        public List<EmployeeLeaveRequestCommentDTO> Comments { get; set; } = new List<EmployeeLeaveRequestCommentDTO>();
    }
}
