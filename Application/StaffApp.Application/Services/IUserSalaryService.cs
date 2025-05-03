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
        Task<GeneralResponseDTO> GenerateEmployeesMonthSalary(int year, int month);
        Task<List<EmployeeSalaryAddonDTO>> GetUnAssignedSalaryAddonsAsync(string userId);
        Task<GeneralResponseDTO> SaveUserSalaryAddonAsync(EmployeeSalaryAddonDTO salaryAddon);
        Task<PaginatedResultDTO<EmployeeSalaryBasicDTO>> GetAllUsersSalariesAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<EmployeeSalarySlipDTO> GetEmployeeEstimateSalarySlipAsync(string userId);
        Task<EmployeeSalarySlipDTO> GetEmployeeSalarySlipAsync(string userId, int year, int month);
        Task<string> GenerateEstimateSalarySlipAsync(EmployeeSalarySlipDTO salarySlip);
        Task<GeneralResponseDTO> CheckEmployeesMonthlySalaryGeneratedAsync(int year, int month);
        Task<PaginatedResultDTO<EmployeeSalarySummaryDTO>> GetMonthlyEmployeeSalaries(int year, int month, int pageNumber, int pageSize, string sortField = null, bool ascending = true);
        Task<GeneralResponseDTO> UpdateUserMonthlySalaryAsync(EmployeeMonthlySalaryDTO salary);
        Task<GeneralResponseDTO> ApproveMonthlySalaryAsBulkAsync(EmployeeMonthlySalaryDTO salary, string comment);
        Task<GeneralResponseDTO> AskToReviseMonthlySalaryAsBulkAsync(EmployeeMonthlySalaryDTO salary, string comment);
        Task<GeneralResponseDTO> UpdateMonthlySalarySubmittedToBankAsBulkAsync(EmployeeMonthlySalaryDTO salary, string comment);
        Task<GeneralResponseDTO> UpdateMonthlySalaryTransferredAsBulkAsync(EmployeeMonthlySalaryDTO salary, string comment);
        Task<EmployeeMonthlySalaryDTO> GetEmployeeMonthlySalary(int id);
    }
}
