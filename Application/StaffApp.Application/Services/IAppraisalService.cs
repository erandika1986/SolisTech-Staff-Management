using StaffApp.Application.DTOs.Appraisal;
using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.Services
{
    public interface IAppraisalService
    {
        Task<GeneralResponseDTO> GenerateAppraisalDataForSelectedCompanyYear(int companyYearId);
        Task<PaginatedResultDTO<AppraisalPeriodDTO>> GetAppraisalPeriodForSelectedYear(int companyYearId);
        Task<List<UserAppraisalSummaryDTO>> GetMyAssignedAppraisal(int companyYearId);
        Task<EmployeeAppraisalDTO> GetEmployeeAppraisalById(int userAppraisalId);
        Task<GeneralResponseDTO> SaveUserAppraisal(EmployeeAppraisalDTO userAppraisalDetail);
        Task<GeneralResponseDTO> CompleteUserAppraisal(EmployeeAppraisalDTO userAppraisalDetail);
        Task<GeneralResponseDTO> GenerateUserRecordsForSelectedAppraisalPeriod(int appraisalPeriodId);
        Task<List<EmployeeAppraisalDTO>> GetEmployeeAppraisalsByEmployeeId();
        Task<DocumentDTO> GenerateEmployeeAppraisalDocumentAsync(int userAppraisalId);

    }
}
