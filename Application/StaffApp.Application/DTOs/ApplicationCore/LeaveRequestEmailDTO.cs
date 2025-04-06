namespace StaffApp.Application.DTOs.ApplicationCore
{
    public class LeaveRequestEmailDTO
    {
        public string RequestId { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmail { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeID { get; set; }
        public string Department { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
    }
}
