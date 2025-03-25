using StaffApp.Domain.Entity;

namespace StaffApp.Application.Services
{
    public interface ILeaveAllocationService
    {
        Task<bool> AssignYearlyLeavesAsync(int year);
        Task<decimal> GetRemainingLeavesAsync(string employeeId, LeaveType leaveType);
        Task<bool> AllocateLeaveAsync(string employeeId, LeaveType leaveType, decimal days);
    }
}
