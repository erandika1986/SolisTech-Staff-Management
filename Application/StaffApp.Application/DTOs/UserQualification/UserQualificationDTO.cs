using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.UserQualification
{
    public class UserQualificationDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        public string UserId { get; set; }


        [Display(Name = "Qualification Name")]
        public string QualificationName { get; set; }


        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }


        [Display(Name = "Original File Name")]
        public string OriginalFileName { get; set; }


        [Display(Name = "Saved File Name")]
        public string SaveFileName { get; set; }

        public string Path { get; set; }


        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
    }
}
