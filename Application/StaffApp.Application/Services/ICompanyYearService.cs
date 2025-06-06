﻿using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.CompanyYear;

namespace StaffApp.Application.Services
{
    public interface ICompanyYearService
    {
        Task<List<CompanyYearDTO>> GetAllCompanyYears(bool isActive = true);
        Task<CompanyYearDTO> GetCompanyYearById(int id);
        Task<GeneralResponseDTO> SaveCompanyYear(CompanyYearDTO model);
        Task<GeneralResponseDTO> DeleteCompanyYear(int id);
        Task<GeneralResponseDTO> SetAsCurrentCompanyYear(int year);
        Task<CompanyYearDTO> GetCurrentYear();
    }
}
