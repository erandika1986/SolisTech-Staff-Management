using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;

namespace StaffApp.Application.Services
{
    public interface IUserSalaryService
    {
        Task<GeneralResponseDTO> SaveUserSalaryAsync(EmployeeSalaryDTO salary);
        Task<EmployeeSalaryDTO> GetEmployeeSalaryByIdAsync(string userId);
        Task<GeneralResponseDTO> ApproveUserSalaryAsync(EmployeeSalaryDTO salary, string comment);
        Task<GeneralResponseDTO> AskToReviseUserSalaryAsync(EmployeeSalaryDTO salary, string comment);
        Task<List<EmployeeSalaryAddonDTO>> GetUnAssignedSalaryAddonsAsync(string userId);
        Task<GeneralResponseDTO> SaveUserSalaryAddonAsync(EmployeeSalaryAddonDTO salaryAddon);
        Task<PaginatedResultDTO<EmployeeSalaryBasicDTO>> GetAllUsersSalariesAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<EmployeeSalarySlipDTO> GetEmployeeEstimateSalarySlip(string userId);
        Task<EmployeeSalarySlipDTO> GetEmployeeSalarySlip(string userId, int year, int month);
        Task<string> GenerateEstimateSalarySlip(EmployeeSalarySlipDTO salarySlip);
    }
}
