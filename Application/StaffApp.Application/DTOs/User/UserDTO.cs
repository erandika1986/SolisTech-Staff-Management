using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Department;

namespace StaffApp.Application.DTOs.User
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string? NICNumber { get; set; }
        public string? LandNumber { get; set; }

        //public string? RoleName { get; set; }


        public string? PhoneNumber { get; set; }
        public DateTime? HireDate { get; set; } = DateTime.Now;
        public DateTime? BirthDate { get; set; } = DateTime.Now;
        public DateTime? ConfirmationDate { get; set; } = null;

        public RoleDTO SelectedRole { get; set; }
        public DropDownDTO SelectedEmploymentType { get; set; }
        public DropDownDTO SelectedMaritalStatus { get; set; }

        public DropDownDTO SelectedGender { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<int> DepartmentIds { get; set; } = new HashSet<int>();
        public IEnumerable<DepartmentDTO> Departments { get; set; } = new HashSet<DepartmentDTO>();
    }
}
