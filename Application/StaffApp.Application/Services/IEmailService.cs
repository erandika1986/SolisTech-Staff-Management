using StaffApp.Application.DTOs.ApplicationCore;

namespace StaffApp.Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentsAsync(string to, string subject, string htmlBody, List<EmailAttachmentDTO> attachments);
        Task SendEmailToMultipleRecipientsAsync(List<string> toAddresses, string subject, string htmlBody, List<EmailAttachmentDTO> attachments = null);
    }
}
