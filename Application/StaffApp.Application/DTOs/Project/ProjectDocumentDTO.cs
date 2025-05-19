namespace StaffApp.Application.DTOs.Project
{
    public class ProjectDocumentDTO
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }
        public string SavedPath { get; set; }
    }

    public class ProjectDocumentAttachmentDTO
    {
        public int ProjectId { get; set; }
        public List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> Files { get; set; } = new List<Microsoft.AspNetCore.Components.Forms.IBrowserFile> { };
    }
}
