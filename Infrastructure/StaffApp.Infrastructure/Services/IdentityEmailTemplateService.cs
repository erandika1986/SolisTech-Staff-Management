using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StaffApp.Infrastructure.Services
{
    public class IdentityEmailTemplateService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public IdentityEmailTemplateService(
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<string> GetConfirmationEmailTemplateAsync(string confirmationLink, string userName)
        {
            string templatePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "EmailConfirmationTemplate.html");
            string template = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            template = template
                .Replace("@Model.Name", userName)
                .Replace("@Model.ApplicationName", _configuration["ApplicationName"])
                .Replace("@Model.ConfirmationUrl", confirmationLink)
                .Replace("{CompanyName}", _configuration["CompanyName"])
                .Replace("{SupportEmail}", _configuration["SupportEmail"]);

            return template;
        }

        public async Task<string> GetPasswordResetTemplateAsync(string resetLink, string userName)
        {
            string templatePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "PasswordResetTemplate.html");
            string template = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            template = template
                .Replace("{UserName}", userName)
                .Replace("{ResetLink}", resetLink)
                .Replace("{CompanyName}", _configuration["CompanyName"])
                .Replace("{SupportEmail}", _configuration["SupportEmail"]);

            return template;
        }

        public async Task<string> GetTwoFactorCodeTemplateAsync(string code, string userName)
        {
            string templatePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "TwoFactorCodeTemplate.html");
            string template = await File.ReadAllTextAsync(templatePath);

            // Replace placeholders
            template = template
                .Replace("{UserName}", userName)
                .Replace("{Code}", code)
                .Replace("{CompanyName}", _configuration["CompanyName"])
                .Replace("{SupportEmail}", _configuration["SupportEmail"]);

            return template;
        }
    }
}
