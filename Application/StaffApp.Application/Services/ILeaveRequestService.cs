using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.EmploymentLeave;

namespace StaffApp.Application.Services
{
    public interface ILeaveRequestService
    {
        Task<GeneralResponseDTO> CreateLeaveRequestAsync(EmployeeLeaveRequestDTO leaveRequest);
        Task<GeneralResponseDTO> ApproveLeaveRequestAsync(int leaveRequestId);
        Task<GeneralResponseDTO> RejectLeaveRequestAsync(int leaveRequestId);
        Task<PaginatedResultDTO<EmployeeLeaveRequestDTO>> GetMyLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus);
    }
}
