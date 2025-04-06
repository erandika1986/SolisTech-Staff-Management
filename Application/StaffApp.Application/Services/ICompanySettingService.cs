using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.CompanySettings;

namespace StaffApp.Application.Services
{
    public interface ICompanySettingService
    {
        Task<CompanyDetailDTO> GetCompanyDetail();
        Task<GeneralResponseDTO> SaveCompanyDetailAsync(CompanyDetailDTO companyDetail);
        Task<GeneralResponseDTO> SaveCompanySMTPSettingAsync(CompanySMTPSettingDTO companySMTPSetting);
        Task<CompanySMTPSettingDTO> GetCompanySMTPSetting();
    }
}
