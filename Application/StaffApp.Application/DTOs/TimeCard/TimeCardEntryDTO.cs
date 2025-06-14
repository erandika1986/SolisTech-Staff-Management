using StaffApp.Domain.Enum;

namespace StaffApp.Application.DTOs.TimeCard
{
    public class TimeCardEntryDTO
    {
        public int Id { get; set; }
        public int TimeCardId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        public double HoursWorked { get; set; }
        public string Notes { get; set; }
        public string ManagerComment { get; set; }
        public TimeCardEntryStatus Status { get; set; }
        public string StatusName { get; set; }
        public bool IsModified { get; set; }
    }
}
