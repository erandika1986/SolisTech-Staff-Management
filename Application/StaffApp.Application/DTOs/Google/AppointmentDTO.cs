﻿namespace StaffApp.Application.DTOs.Google
{
    public class AppointmentDTO
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsBlock { get; set; }
        public bool IsReadonly { get; set; }
        public string RecurrenceID { get; set; }
        public string FollowingID { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public List<string> EventAttendees { get; set; } = new();
    }
}
