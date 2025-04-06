namespace StaffApp.Application.DTOs.Google
{
    public class CalendarEventDTO
    {
        public string Summary { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; }
        public List<string> EventAttendees { get; set; } = new();
    }
}
