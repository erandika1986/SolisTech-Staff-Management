namespace StaffApp.Application.DTOs.UserQualification
{
    public class EmployeeDocumentCategoryContainerDTO
    {
        public DocumentCategoryDTO DocumentCategory { get; set; }
        public List<EmployeeDocumentDTO> EmployeeDocuments { get; set; } = new();
    }
}
