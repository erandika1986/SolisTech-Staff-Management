using StaffApp.Application.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Project
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DropDownDTO SelectedManagementPlatform { get; set; }

        [Display(Name = "Management Platform")]
        public string ManagementPlatformName { get; set; }


        public DateTime? StartDate { get; set; }

        [Display(Name = "Start Date")]
        public string StartDateText { get; set; }


        public DateTime? EndDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDateText { get; set; }

        [Display(Name = "Manager Id")]
        public UserDropDownDTO SelectedManager { get; set; }

        [Display(Name = "Project Manager")]
        public string ManagerName { get; set; }
        public DropDownDTO SelectedStatus { get; set; }

        [Display(Name = "Status")]
        public string StatusName { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Client Address")]
        public string ClientAddress { get; set; }

        [Display(Name = "Client Phone")]
        public string ClientPhone { get; set; }

        [Display(Name = "Client Email")]
        public string ClientEmail { get; set; }

        public List<ProjectMemberDTO> ProjectMembers { get; set; } = new List<ProjectMemberDTO>();
        public List<ProjectDocumentDTO> Projects { get; set; } = new List<ProjectDocumentDTO>();
    }
}
