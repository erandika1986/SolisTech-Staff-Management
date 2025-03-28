using StaffApp.Application.DTOs.Common;
using StaffApp.Domain.Entity;

namespace StaffApp.Application.Services
{
    public interface ILeaveAllocationService
    {
        Task<GeneralResponseDTO> AssignYearlyLeavesAsync(int year);
        Task<decimal> GetRemainingLeavesAsync(string employeeId, int leaveTypeId);
        Task<GeneralResponseDTO> AllocateLeaveAsync(string employeeId, LeaveType leaveType, decimal days);
    }
}
