using StaffApp.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace StaffApp.Application.DTOs.Project
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectManagementPlatform ManagementPlatform { get; set; }

        [Display(Name = "Management Platform")]
        public string ManagementPlatformName { get; set; }


        public DateTime? StartDate { get; set; }

        [Display(Name = "Start Date")]
        public string StartDateText { get; set; }


        public DateTime? EndDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDateText { get; set; }

        [Display(Name = "Manager Id")]
        public string ManagerId { get; set; }

        [Display(Name = "Project Manager")]
        public string ManagerName { get; set; }
        public ProjectStatus Status { get; set; }
        [Display(Name = "Status")]
        public string StatusName { get; set; }

        public List<ProjectMemberDTO> ProjectMembers { get; set; } = new List<ProjectMemberDTO>();
        public List<ProjectDocumentDTO> Projects { get; set; } = new List<ProjectDocumentDTO>();
    }
}
