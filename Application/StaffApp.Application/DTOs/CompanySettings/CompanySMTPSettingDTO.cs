namespace StaffApp.Application.DTOs.CompanySettings
{
    public class CompanySMTPSettingDTO
    {
        public string SMTPEnableSsl { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPPort { get; set; }
        public string SMTPSenderEmail { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPUsername { get; set; }
    }
}
