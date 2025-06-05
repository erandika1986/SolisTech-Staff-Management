using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.TimeCard
{
    public class TimeCardDTO
    {
        public int Id { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public string DateByString { get; set; }
        public string Notes { get; set; }
        public TimeCardStatus Status { get; set; }
        public string StatusName { get; set; }
        public string? ManagerComment { get; set; }

        public List<TimeCardEntryDTO> TimeCardEntries { get; set; } = new List<TimeCardEntryDTO>();
    }
}
