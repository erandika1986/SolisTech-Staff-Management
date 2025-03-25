using StaffApp.Domain.Entity;

namespace StaffApp.Application.Services
{
    public interface ILeaveRequestService
    {
        Task<bool> CreateLeaveRequestAsync(EmployeeLeaveRequest leaveRequest);
        Task<bool> ApproveLeaveRequestAsync(int leaveRequestId);
        Task<bool> RejectLeaveRequestAsync(int leaveRequestId);
    }
}
