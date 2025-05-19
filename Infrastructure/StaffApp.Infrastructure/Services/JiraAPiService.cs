using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StaffApp.Infrastructure.Services
{
    public class JiraAPiService
    {
        private const string BaseUrl = "https://prodigysoft.atlassian.net/rest/api/3";
        private const string Username = "erandika1986@gmail.com";
        private const string ApiToken = "ATATT3xFfGF0XVVGGhJ02xYVpf2n-s2K-GhPlr9w6i_JKu98tM9hq5lEMP3Qln-tHOWBV7dBq3oU_AsodUMoa3pWJhE9Dyij4N4_F9ev5KZoTnfzE2Wm1foyc6_lvWpkrvFW49wZs0IPSqqAwi-DBhVBNJuJH3XrJFNWH8AFHP2tlzDSMigNm2A=496DA069";


        static async Task<List<JiraProject>> GetJiraProjects()
        {
            using var httpClient = new HttpClient();

            // Set base address
            httpClient.BaseAddress = new Uri(BaseUrl);

            // Set Basic Authentication header
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{ApiToken}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // Set appropriate headers
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Make the request
            var response = await httpClient.GetAsync("/project");

            // Ensure successful response
            response.EnsureSuccessStatusCode();

            // Parse response content
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var projects = JsonSerializer.Deserialize<List<JiraProject>>(content, options);

            return projects ?? new List<JiraProject>();
        }
    }


    // JIRA Project model class
    public class JiraProject
    {
        public string Id { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProjectTypeKey { get; set; } = string.Empty;
        public ProjectCategory? ProjectCategory { get; set; }
        public AvatarUrls? AvatarUrls { get; set; }
    }

    public class ProjectCategory
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AvatarUrls
    {
        public string Small { get; set; } = string.Empty;
        public string Medium { get; set; } = string.Empty;
        public string Large { get; set; } = string.Empty;
    }
}
