using StaffApp.Application.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Department
{
    public class DepartmentDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Department Head Id")]
        public string? DepartmentHeadId { get; set; }
        public UserDropDownDTO DepartmentHead { get; set; }

        [Display(Name = "Department Head")]
        public string DepartmentHeadName { get; set; }

        [Display(Name = "Employee Count")]
        public int AssignedEmployeeCount { get; set; }
    }
}
