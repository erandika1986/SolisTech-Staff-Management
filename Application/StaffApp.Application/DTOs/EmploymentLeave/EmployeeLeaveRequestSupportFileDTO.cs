namespace StaffApp.Application.DTOs.EmploymentLeave
{
    public class EmployeeLeaveRequestSupportFileDTO
    {
        public int Id { get; set; }
        public int EmployeeLeaveRequestId { get; set; }
        public string OriginalFileName { get; set; }
        public string SavedFileName { get; set; }
        public string SaveFileURL { get; set; }
    }
}
