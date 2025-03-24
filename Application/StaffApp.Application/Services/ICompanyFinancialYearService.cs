using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.CompanyYear;

namespace StaffApp.Application.Services
{
    public interface ICompanyFinancialYearService
    {
        Task<List<CompanyYearDTO>> GetAllCompanyYears(bool isActive = true);
        Task<CompanyYearDTO> GetCompanyYearById(int id);
        Task<GeneralResponseDTO> SaveCompanyYear(CompanyYearDTO model);
        Task<GeneralResponseDTO> DeleteCompanyYear(int id);
    }
}
