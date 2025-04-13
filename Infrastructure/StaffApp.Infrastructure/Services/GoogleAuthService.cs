using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StaffApp.Application.DTOs.Google;
using System.Text.Json;

namespace StaffApp.Infrastructure.Services
{
    public class GoogleAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public GoogleAuthService(IHttpClientFactory httpClientFactory, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAuthorizationUrlAsync()
        {

            var user = _httpContextAccessor.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return null; // Handle unauthenticated users
            }

            var accessToken = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];



            var redirectUri = _config["Authentication:Google:RedirectUri"];
            var clientId = _config["Authentication:Google:ClientId"];
            var scope = "https://www.googleapis.com/auth/calendar.events";

            return $"https://accounts.google.com/o/oauth2/v2/auth?" +
                   $"response_type=code&client_id={clientId}&redirect_uri={redirectUri}&" +
                   $"scope={scope}&access_type=offline&prompt=consent";
        }

        public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var client = _httpClientFactory.CreateClient();

            var data = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _config["Authentication:Google:ClientId"],
                ["client_secret"] = _config["Authentication:Google:ClientSecret"],
                ["redirect_uri"] = _config["Authentication:Google:RedirectUri"],
                ["grant_type"] = "authorization_code"
            };

            var response = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(data));
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }


        //public async Task CreateCalendarEventAsync(string accessToken, string summary, string description, DateTime start, DateTime end)
        //{
        //    var credential = GoogleCredential.FromAccessToken(accessToken);
        //    var service = new CalendarService(new BaseClientService.Initializer
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "My Blazor App"
        //    });

        //    var calendarEvent = new Event
        //    {
        //        Summary = summary,
        //        Description = description,
        //        Start = new EventDateTime { DateTime = start, TimeZone = "Asia/Colombo" },
        //        End = new EventDateTime { DateTime = end, TimeZone = "Asia/Colombo" }
        //    };

        //    await service.Events.Insert(calendarEvent, "primary").ExecuteAsync();
        //}

        public List<AppointmentDTO> GetEvents(DateTime date, string accessToken)
        {
            try
            {
                var service = CreateCalenderService(accessToken);

                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = date;
                request.MaxResults = 2500;
                Events events = request.Execute();
                List<AppointmentDTO> eventDatas = new List<AppointmentDTO>();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (Event eventItem in events.Items)
                    {
                        if (eventItem.Start == null && eventItem.Status == "cancelled")
                        {
                            continue;
                        }
                        DateTime start;
                        DateTime end;
                        if (string.IsNullOrEmpty(eventItem.Start.Date))
                        {
                            start = (DateTime)eventItem.Start.DateTime;
                            end = (DateTime)eventItem.End.DateTime;
                        }
                        else
                        {
                            start = Convert.ToDateTime(eventItem.Start.Date);
                            end = Convert.ToDateTime(eventItem.End.Date);
                        }
                        AppointmentDTO eventData = new AppointmentDTO()
                        {
                            Id = eventItem.Id,
                            Subject = eventItem.Summary,
                            StartTime = start,
                            EndTime = end,
                            Location = eventItem.Location,
                            Description = eventItem.Description,
                            IsAllDay = !string.IsNullOrEmpty(eventItem.Start.Date)
                        };
                        eventDatas.Add(eventData);
                    }
                }
                return eventDatas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<AppointmentDTO>();
            }
        }

        public void InsertEvent(AppointmentDTO eventData, string accessToken)
        {
            try
            {
                var service = CreateCalenderService(accessToken);

                EventDateTime start;
                EventDateTime end;
                if (eventData.IsAllDay)
                {
                    start = new EventDateTime() { Date = eventData.StartTime.ToString("yyyy-MM-dd"), TimeZone = eventData.StartTimezone };
                    end = new EventDateTime() { Date = eventData.EndTime.ToString("yyyy-MM-dd"), TimeZone = eventData.EndTimezone };
                }
                else
                {
                    start = new EventDateTime() { DateTime = eventData.StartTime, TimeZone = eventData.StartTimezone };
                    end = new EventDateTime() { DateTime = eventData.EndTime, TimeZone = eventData.EndTimezone };
                }
                Event eventItem = new Event()
                {
                    Summary = eventData.Subject,
                    Start = start,
                    End = end,
                    Location = eventData.Location,
                    Description = eventData.Description,
                    Created = DateTime.Now.ToUniversalTime(),
                    Attendees = eventData.EventAttendees.Select(x => new EventAttendee() { Email = x }).ToList(),
                };
                EventsResource.InsertRequest addEvent = service.Events.Insert(eventItem, "primary");
                addEvent.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;
                addEvent.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void UpdateEvent(AppointmentDTO eventData, string accessToken)
        {
            try
            {
                var service = CreateCalenderService(accessToken);

                var editedEvent = service.Events.Get("primary", eventData.Id).Execute();
                editedEvent.Summary = eventData.Subject;
                editedEvent.Location = eventData.Location;
                editedEvent.Description = eventData.Description;
                editedEvent.Updated = DateTime.Now.ToUniversalTime();
                if (eventData.IsAllDay)
                {
                    editedEvent.Start = new EventDateTime() { Date = eventData.StartTime.ToString("yyyy-MM-dd") };
                    editedEvent.End = new EventDateTime() { Date = eventData.EndTime.ToString("yyyy-MM-dd") };
                }
                else
                {
                    editedEvent.Start = new EventDateTime() { DateTime = eventData.StartTime };
                    editedEvent.End = new EventDateTime() { DateTime = eventData.EndTime };
                }
                var updatedEvent = service.Events.Update(editedEvent, "primary", editedEvent.Id);
                updatedEvent.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveEvent(string id, string accessToken)
        {
            try
            {
                var service = CreateCalenderService(accessToken);

                EventsResource.DeleteRequest deleteEvent = service.Events.Delete("primary", id);
                deleteEvent.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private CalendarService CreateCalenderService(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SolisTech Staff"
            });

            return service;
        }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public string TokenType { get; set; } = "";
        public int ExpiresIn { get; set; }
        public string IdToken { get; set; } = "";
    }
}
