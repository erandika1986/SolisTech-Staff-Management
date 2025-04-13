namespace StaffApp.Application.DTOs.CompanySettings
{
    public class CompanyDetailDTO
    {
        public string ApplicationUrl { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string CompanyName { get; set; }
        public string LeaveRequestCCList { get; set; }
        public bool IsPasswordLoginEnable { get; set; }
    }
}
