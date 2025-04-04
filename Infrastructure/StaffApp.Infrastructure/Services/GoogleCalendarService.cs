using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly CalendarService calendarService;

        public GoogleCalendarService(CalendarService calendarService)
        {
            this.calendarService = calendarService;
        }
        public async Task<string> CreateEventWithAttendeesAsync(string title, string description, DateTime startTime, DateTime endTime, string location, List<string> attendeeEmails)
        {
            var newEvent = new Event()
            {
                Summary = title,
                Description = description,
                Location = location,
                Start = new EventDateTime()
                {
                    DateTime = startTime,
                    TimeZone = TimeZoneInfo.Local.Id
                },
                End = new EventDateTime()
                {
                    DateTime = endTime,
                    TimeZone = TimeZoneInfo.Local.Id
                },
                Attendees = attendeeEmails.Select(email => new EventAttendee { Email = email }).ToList(),

                //SendUpdates = "all"
            };

            var createdEvent = await calendarService.Events.Insert(newEvent, "primary").ExecuteAsync();
            //createdEvent. = EventsResource.InsertRequest.SendUpdatesEnum.All;


            return createdEvent.HtmlLink;
        }
    }
}
