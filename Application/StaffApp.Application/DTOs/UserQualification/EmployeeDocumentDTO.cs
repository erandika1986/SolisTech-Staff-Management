namespace StaffApp.Application.DTOs.UserQualification
{
    public class EmployeeDocumentDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string DocumentCategory { get; set; } = "";
        public string OriginalFileName { get; set; } = "";
        public string SaveFileName { get; set; } = "";
        public string FileType { get; set; } = "";
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Path { get; set; } = "";
    }
}
