using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.User
{
    public class RoleDTO
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Is Manager Type Role")]
        public bool IsManagerTypeRole { get; set; }

        [Display(Name = "Default Hourly Rate")]
        public double DefaultHourlyRate { get; set; }

        [Display(Name = "Manager Type Role")]
        public string ManagerTypeRole { get; set; }
    }
}
