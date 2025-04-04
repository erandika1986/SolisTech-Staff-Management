namespace StaffApp.Application.Services
{
    public interface IGoogleCalendarService
    {
        Task<string> CreateEventWithAttendeesAsync(
        string title,
        string description,
        DateTime startTime,
        DateTime endTime,
        string location,
        List<string> attendeeEmails);
    }
}
