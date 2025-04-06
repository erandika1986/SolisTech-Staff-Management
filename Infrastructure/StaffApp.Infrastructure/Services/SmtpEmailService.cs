using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.DTOs.ApplicationCore;
using StaffApp.Application.Services;
using System.Net;
using System.Net.Mail;

namespace StaffApp.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly IWebHostEnvironment _environment;

        public SmtpEmailService(
            IConfiguration configuration,
            ILogger<SmtpEmailService> logger,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            await SendEmailToMultipleRecipientsAsync(new List<string> { to }, subject, htmlBody);
        }

        public async Task SendEmailWithAttachmentsAsync(string to, string subject, string htmlBody, List<EmailAttachmentDTO> attachments)
        {
            await SendEmailToMultipleRecipientsAsync(new List<string> { to }, subject, htmlBody, attachments);
        }

        public async Task SendEmailToMultipleRecipientsAsync(List<string> toAddresses, string subject, string htmlBody, List<EmailAttachmentDTO> attachments = null)
        {
            if (toAddresses == null || toAddresses.Count == 0)
            {
                throw new ArgumentException("At least one recipient email address is required", nameof(toAddresses));
            }

            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = int.Parse(smtpSettings["Port"]);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                var senderEmail = smtpSettings["SenderEmail"];
                var senderName = smtpSettings["SenderName"];
                var replyToEmail = smtpSettings["ReplyToEmail"] ?? senderEmail;
                var replyToName = smtpSettings["ReplyToName"] ?? senderName;

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = enableSsl;
                    client.Timeout = 30000; // 30 seconds timeout

                    using (var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName),
                        Subject = subject,
                        Body = htmlBody,
                        IsBodyHtml = true,
                        Priority = MailPriority.Normal
                    })
                    {
                        // Add reply-to address if different from sender
                        if (replyToEmail != senderEmail || replyToName != senderName)
                        {
                            mailMessage.ReplyToList.Add(new MailAddress(replyToEmail, replyToName));
                        }

                        // Add all recipients
                        foreach (var address in toAddresses)
                        {
                            mailMessage.To.Add(address);
                        }

                        // Add CC recipients if configured
                        var ccAddresses = smtpSettings["CcAddresses"];
                        if (!string.IsNullOrEmpty(ccAddresses))
                        {
                            foreach (var ccAddress in ccAddresses.Split(';', StringSplitOptions.RemoveEmptyEntries))
                            {
                                mailMessage.CC.Add(ccAddress.Trim());
                            }
                        }

                        // Add BCC recipients if configured
                        var bccAddresses = smtpSettings["BccAddresses"];
                        if (!string.IsNullOrEmpty(bccAddresses))
                        {
                            foreach (var bccAddress in bccAddresses.Split(';', StringSplitOptions.RemoveEmptyEntries))
                            {
                                mailMessage.Bcc.Add(bccAddress.Trim());
                            }
                        }

                        // Add attachments if any
                        if (attachments != null && attachments.Count > 0)
                        {
                            foreach (var attachment in attachments)
                            {
                                var stream = new MemoryStream(attachment.Content);
                                mailMessage.Attachments.Add(new Attachment(stream, attachment.FileName, attachment.ContentType));
                            }
                        }

                        // Send the email
                        await client.SendMailAsync(mailMessage);
                        _logger.LogInformation($"Email sent successfully to {string.Join(", ", toAddresses)}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                throw new ApplicationException($"Failed to send email: {ex.Message}", ex);
            }
        }
    }

    // Extended service for leave requests with attachment support
    public class LeaveRequestEmailService
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LeaveRequestEmailService> _logger;
        private readonly IWebHostEnvironment _environment;

        public LeaveRequestEmailService(
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<LeaveRequestEmailService> logger,
            IWebHostEnvironment environment)
        {
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        public async Task SendLeaveRequestEmailAsync(LeaveRequestEmailDTO model, List<IFormFile> attachments = null)
        {
            try
            {
                string templatePath = Path.Combine(_environment.ContentRootPath, "EmailTemplates", "LeaveRequestTemplate.html");
                string htmlBody = File.ReadAllText(templatePath);

                // Replace placeholders with actual values
                htmlBody = htmlBody
                    .Replace("{ManagerName}", model.ManagerName)
                    .Replace("{EmployeeName}", model.EmployeeName)
                    .Replace("{EmployeeID}", model.EmployeeID)
                    .Replace("{Department}", model.Department)
                    .Replace("{LeaveType}", model.LeaveType)
                    .Replace("{StartDate}", model.StartDate.ToString("dd-MMM-yyyy"))
                    .Replace("{EndDate}", model.EndDate.ToString("dd-MMM-yyyy"))
                    .Replace("{TotalDays}", model.TotalDays.ToString())
                    .Replace("{Reason}", model.Reason)
                    .Replace("{ApproveLink}", $"{_configuration["ApplicationUrl"]}/leave/approve/{model.RequestId}")
                    .Replace("{RejectLink}", $"{_configuration["ApplicationUrl"]}/leave/reject/{model.RequestId}")
                    .Replace("{CurrentYear}", DateTime.Now.Year.ToString())
                    .Replace("{CompanyName}", _configuration["CompanyName"]);

                // Process attachments if any
                List<EmailAttachmentDTO> emailAttachments = null;
                if (attachments != null && attachments.Count > 0)
                {
                    emailAttachments = new List<EmailAttachmentDTO>();
                    foreach (var file in attachments)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            emailAttachments.Add(new EmailAttachmentDTO
                            {
                                FileName = file.FileName,
                                Content = memoryStream.ToArray(),
                                ContentType = file.ContentType
                            });
                        }
                    }
                }

                // Send email with attachments
                await _emailService.SendEmailWithAttachmentsAsync(
                    model.ManagerEmail,
                    $"Leave Request from {model.EmployeeName}",
                    htmlBody,
                    emailAttachments);

                _logger.LogInformation($"Leave request email sent to {model.ManagerEmail} for employee {model.EmployeeName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send leave request email: {ex.Message}");
                throw;
            }
        }
    }
}
