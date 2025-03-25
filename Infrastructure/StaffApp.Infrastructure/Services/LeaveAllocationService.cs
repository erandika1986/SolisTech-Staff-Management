using StaffApp.Application.Contracts;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveAllocationService(IStaffAppDbContext staffAppDbContext) : ILeaveAllocationService
    {
        public async Task<bool> AllocateLeaveAsync(string employeeId, LeaveType leaveType, decimal days)
        {
            var employeea = staffAppDbContext.ApplicationUsers.Where(x => x.IsActive == true);

            foreach (var employee in employeea)
            {
                var leaveConfigurations = staffAppDbContext.LeaveTypesConfigs.Where(x => x.EmployeeTypeId == employee.EmployeeTypeId);
            }

            return true;
        }

        public Task<bool> AssignYearlyLeavesAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetRemainingLeavesAsync(string employeeId, LeaveType leaveType)
        {
            throw new NotImplementedException();
        }
    }
}
