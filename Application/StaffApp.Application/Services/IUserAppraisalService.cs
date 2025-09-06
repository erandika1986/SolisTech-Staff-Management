using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.UserAppraisal;

namespace StaffApp.Application.Services
{
    public interface IUserAppraisalService
    {
        Task<GeneralResponseDTO> GenerateUserRecordsForSelectedAppraisalPeriod(int appraisalPeriodId);
        Task<List<UserAppraisalSummaryDTO>> GetUsersAppraisalsByAppraisalPeriodId(int appraisalPeriodId);
        Task<UserAppraisalDTO> GetUserAppraisalDetailsById(int userAppraisalId);
        Task<GeneralResponseDTO> SaveUserAppraisal(UserAppraisalDTO userAppraisalDetail);
        Task<GeneralResponseDTO> CompleteUserAppraisal(int userAppraisalId);
        Task<StaffApp.Application.DTOs.Appraisal.EmployeeAppraisalDTO> GetEmployeeAppraisalById(int userAppraisalId);
    }
}
