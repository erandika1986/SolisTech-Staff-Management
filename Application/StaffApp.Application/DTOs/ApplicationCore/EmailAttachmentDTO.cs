namespace StaffApp.Application.DTOs.ApplicationCore
{
    public class EmailAttachmentDTO
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}
