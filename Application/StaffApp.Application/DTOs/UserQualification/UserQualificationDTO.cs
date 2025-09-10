using StaffApp.Application.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.UserQualification
{
    public class UserQualificationDTO
    {
        [Display(Name = "Id")]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DocumentCategoryDTO SelectedDocumentCategory { get; set; }
        public DropDownDTO SelectedDocumentName { get; set; }
        public string OtherName { get; set; }
        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
    }
}
