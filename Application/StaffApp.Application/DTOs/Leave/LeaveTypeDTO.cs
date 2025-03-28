using StaffApp.Application.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Leave
{
    public class LeaveTypeDTO
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Default Day Count")]
        public int DefaultDays { get; set; }
        public bool HasLeaveTypeLogic { get; set; }


        [Display(Name = "Has Leave Type Logic")]
        public string LeaveTypeLogic { get; set; }

        //public int AllowGenderType { get; set; }
        public DropDownDTO SelectedAllowGenderType { get; set; }

        [Display(Name = "Allow Gender Type")]
        public string AllowGenderTypeName { get; set; }
    }
}
