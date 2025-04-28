using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class IdentityEmailSenderService(IEmailService _emailService, ILogger<IdentityEmailSenderService> _logger) : IEmailSender
    {
        //private readonly IEmailService _emailService;
        //private readonly ILogger<IdentityEmailSenderService> _logger;

        //public IdentityEmailSenderService(IEmailService emailService, ILogger<IdentityEmailSenderService> logger)
        //{
        //    _emailService = emailService;
        //    _logger = logger;
        //}

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Sending identity email to {email}");
            await _emailService.SendEmailAsync(email, subject, htmlMessage);
        }
    }
}
