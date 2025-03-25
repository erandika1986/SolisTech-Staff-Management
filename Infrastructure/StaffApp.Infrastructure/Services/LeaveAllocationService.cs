using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveAllocationService(IStaffAppDbContext staffAppDbContext) : ILeaveAllocationService
    {

        public async Task<bool> AssignYearlyLeavesAsync(int year)
        {
            var employeea = staffAppDbContext.ApplicationUsers.Where(x => x.IsActive == true);

            foreach (var employee in employeea)
            {
                var leaveConfigurations = await staffAppDbContext
                    .LeaveTypesConfigs.Where(x => x.EmployeeTypeId == employee.EmployeeTypeId)
                    .ToListAsync();

                foreach (var leaveConfig in leaveConfigurations)
                {
                    // Calculate leave days based on service and employee type
                    decimal leaveDays = CalculateLeaveDays(employee, leaveConfig, year);

                    if (leaveDays > 0)
                    {
                        // Create or update leave allocation
                        await CreateOrUpdateLeaveAllocation(employee.Id, leaveConfig.LeaveType, year, leaveDays);
                    }
                }
            }

            return true;
        }

        public async Task<decimal> GetRemainingLeavesAsync(string employeeId, LeaveType leaveType)
        {
            var allocation = await staffAppDbContext.EmployeeLeaveAllocations
                .FirstOrDefaultAsync(la =>
                    la.EmployeeId == employeeId &&
                    la.LeaveTypeId == leaveType.Id &&
                    la.CompanyYearId == DateTime.Now.Year);

            return allocation?.RemainingLeaveCount ?? 0;
        }

        public async Task<bool> AllocateLeaveAsync(string employeeId, LeaveType leaveType, decimal days)
        {
            var allocation = await staffAppDbContext.EmployeeLeaveAllocations
                .FirstOrDefaultAsync(la =>
                    la.EmployeeId == employeeId &&
                    la.LeaveType == leaveType &&
                    la.CompanyYearId == DateTime.Now.Year);

            if (allocation == null)
                return false;

            if (allocation.RemainingLeaveCount < days)
                return false;

            allocation.RemainingLeaveCount -= days;
            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);
            return true;
        }

        private decimal CalculateLeaveDays(ApplicationUser employee, LeaveTypeConfig leaveConfig, int year)
        {
            // If minimum service months are not met, return 0
            if (leaveConfig.MinimumServiceMonthsRequired.HasValue)
            {
                int monthsOfService = CalculateMonthsOfService(employee, year);
                if (monthsOfService < leaveConfig.MinimumServiceMonthsRequired.Value)
                    return 0;
            }

            // Pro-rata calculation for partial years
            decimal proRataFactor = CalculateProRataFactor(employee, year);

            // Base leave allocation
            decimal baseLeaveDays = leaveConfig.AnnualLeaveAllowance;

            // Apply pro-rata factor
            return baseLeaveDays * proRataFactor;
        }

        private int CalculateMonthsOfService(ApplicationUser employee, int year)
        {
            // Calculate months of service for the given year
            DateTime yearStart = new DateTime(year, 1, 1);
            DateTime yearEnd = new DateTime(year, 12, 31);

            // Ensure join date is not after year end
            if (employee.HireDate > yearEnd)
                return 0;

            // Calculate months from join date to year end
            DateTime serviceStartDate = employee.HireDate.Value > yearStart
                ? employee.HireDate.Value
                : yearStart;

            return (yearEnd.Year - serviceStartDate.Year) * 12 +
                   (yearEnd.Month - serviceStartDate.Month) + 1;
        }

        private decimal CalculateProRataFactor(ApplicationUser employee, int year)
        {
            // Calculate pro-rata factor based on months of service
            int monthsOfService = CalculateMonthsOfService(employee, year);
            return monthsOfService / 12m;
        }

        private async Task<bool> CreateOrUpdateLeaveAllocation(
            string employeeId,
            LeaveType leaveType,
            int year,
            decimal leaveDays)
        {
            // Check if allocation already exists
            var existingAllocation = await staffAppDbContext.EmployeeLeaveAllocations
                .FirstOrDefaultAsync(la =>
                    la.EmployeeId == employeeId &&
                    la.LeaveType == leaveType &&
                    la.CompanyYearId == year);

            if (existingAllocation != null)
            {
                // Update existing allocation
                existingAllocation.AllocatedLeaveCount = leaveDays;
                existingAllocation.RemainingLeaveCount = leaveDays;
            }
            else
            {
                // Create new allocation
                var newAllocation = new EmployeeLeaveAllocation
                {
                    EmployeeId = employeeId,
                    LeaveType = leaveType,
                    CompanyYearId = year,
                    AllocatedLeaveCount = leaveDays,
                    RemainingLeaveCount = leaveDays
                };
                staffAppDbContext.EmployeeLeaveAllocations.Add(newAllocation);
            }

            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);
            return true;
        }


    }
}
