using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.User
{
    public record BasicUserDTO
    {
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Display(Name = "NIC")]
        public string NIC { get; set; }

        [Display(Name = "Mobile No")]
        public string Phone { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "Hire Date")]
        public string HireDate { get; set; }

        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }
    }
}
