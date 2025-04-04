using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.EmploymentLeave;

namespace StaffApp.Application.Services
{
    public interface ILeaveRequestService
    {
        Task<GeneralResponseDTO> CreateLeaveRequestAsync(EmployeeLeaveRequestDTO leaveRequest);
        Task<GeneralResponseDTO> ApproveLeaveRequestAsync(int leaveRequestId, string comment);
        Task<GeneralResponseDTO> RejectLeaveRequestAsync(int leaveRequestId, string comment);
        Task<GeneralResponseDTO> DeleteLeaveRequestAsync(int leaveRequestId, string comment);
        Task<GeneralResponseDTO> RemoveSavedSupportFile(EmployeeLeaveRequestSupportFileDTO file);
        Task<EmployeeLeaveRequestDTO> GetLeaveRequestById(int leaveRequestId);
        Task<List<UserDropDownDTO>> GetMyReportingManagers();
        Task<PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>> GetMyLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus,
            DateTime? startDate, DateTime? endDate);

        Task<PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>> GetMyAssignedLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus,
            DateTime? startDate, DateTime? endDate);
    }
}
