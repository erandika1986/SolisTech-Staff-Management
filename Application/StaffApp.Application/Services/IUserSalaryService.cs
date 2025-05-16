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
        Task<List<EmployeeMonthlySalaryAddonDTO>> GetUnAssignedMonthlySalaryAddonsAsync(int employeeMonthlySalaryId);
        Task<GeneralResponseDTO> SaveUserSalaryAddonAsync(EmployeeSalaryAddonDTO salaryAddon);
        Task<PaginatedResultDTO<EmployeeSalaryBasicDTO>> GetAllUsersSalariesAsync(int pageNumber, int pageSize, int status, string searchString = null, string sortField = null, bool ascending = true);
        Task<EmployeeSalarySlipDTO> GetEmployeeEstimateSalarySlipAsync(string userId);
        Task<EmployeeSalarySlipDTO> GetEmployeeSalarySlipAsync(int employeeMonthlySalaryId);
        Task<DocumentDTO> GenerateEstimateSalarySlipAsync(EmployeeSalarySlipDTO salarySlip);
        Task<GeneralResponseDTO> CheckEmployeesMonthlySalaryGeneratedAsync(int year, int month);
        Task<PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>> GetMonthlyEmployeeSalaries(int year, int month, int pageNumber, int pageSize, string sortField = null, bool ascending = true);
        Task<PaginatedResultDTO<EmployeeMonthlySalarySummaryDTO>> GetMyMonthlySalaryList(int pageNumber, int pageSize, string sortField = null, bool ascending = true);
        Task<GeneralResponseDTO> UpdateUserMonthlySalaryAsync(EmployeeMonthlySalaryDTO salary);
        Task<GeneralResponseDTO> SubmitMonthlySalaryForApprovalAsBulkAsync(int monthlySalaryId, string comment);
        Task<GeneralResponseDTO> ApproveMonthlySalaryAsBulkAsync(int monthlySalaryId, string comment);
        Task<GeneralResponseDTO> AskToReviseMonthlySalaryAsBulkAsync(int monthlySalaryId, string comment);
        Task<GeneralResponseDTO> UpdateMonthlySalarySubmittedToBankAsBulkAsync(int monthlySalaryId, string comment);
        Task<GeneralResponseDTO> UpdateMonthlySalaryTransferredAsBulkAsync(int monthlySalaryId, string comment);
        Task<EmployeeMonthlySalaryDTO> GetEmployeeMonthlySalary(int id);
        Task<EmployeeMonthlySalaryStatusDTO> GetEmployeeMonthlySalaryStatus(int year, int month);
    }
}
