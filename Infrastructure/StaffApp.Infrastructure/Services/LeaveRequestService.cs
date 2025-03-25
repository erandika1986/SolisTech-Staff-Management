using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        public Task<bool> ApproveLeaveRequestAsync(int leaveRequestId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateLeaveRequestAsync(EmployeeLeaveRequest leaveRequest)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RejectLeaveRequestAsync(int leaveRequestId)
        {
            throw new NotImplementedException();
        }
    }
}
