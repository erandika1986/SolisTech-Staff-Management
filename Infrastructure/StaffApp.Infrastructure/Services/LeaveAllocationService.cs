using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveAllocationService(IStaffAppDbContext staffAppDbContext, ICurrentUserService currentUserService) : ILeaveAllocationService
    {

        public async Task<GeneralResponseDTO> AssignYearlyLeavesAsync(int year)
        {
            try
            {
                var employees = await staffAppDbContext.ApplicationUsers.Where(x => x.IsActive == true).ToListAsync(); ;

                foreach (var employee in employees)
                {
                    var leaveConfigurations = await staffAppDbContext
                        .LeaveTypesConfigs.Where(x => x.EmployeeTypeId == employee.EmployeeTypeId)
                        .ToListAsync();

                    foreach (var leaveConfig in leaveConfigurations)
                    {
                        // Calculate leave days based on service and employee type
                        decimal leaveDays = CalculateLeaveDays(employee, leaveConfig, year);

                        if (leaveDays >= 0)
                        {
                            // Create or update leave allocation
                            await CreateOrUpdateLeaveAllocation(employee.Id, leaveConfig.LeaveType, year, leaveDays);
                        }
                    }
                }

                return new GeneralResponseDTO() { Flag = true, Message = "All the employees updated with assign yearly leave plan." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        public async Task<decimal> GetRemainingLeavesAsync(string employeeId, int leaveTypeId)
        {
            var allocation = await staffAppDbContext.EmployeeLeaveAllocations
                .FirstOrDefaultAsync(la =>
                    la.EmployeeId == employeeId &&
                    la.LeaveTypeId == leaveTypeId &&
                    la.CompanyYearId == DateTime.Now.Year);

            return allocation?.RemainingLeaveCount ?? 0;
        }

        public async Task<GeneralResponseDTO> AllocateLeaveAsync(string employeeId, LeaveType leaveType, decimal days)
        {
            var allocation = await staffAppDbContext.EmployeeLeaveAllocations
                .FirstOrDefaultAsync(la =>
                    la.EmployeeId == employeeId &&
                    la.LeaveType == leaveType &&
                    la.CompanyYearId == DateTime.Now.Year);

            if (allocation == null)
                return new GeneralResponseDTO() { Flag = false, Message = "Not allocation found." };

            if (allocation.RemainingLeaveCount < days)
                return new GeneralResponseDTO() { Flag = false, Message = "Not enough remaining leave count." };

            allocation.RemainingLeaveCount -= days;
            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);
            return new GeneralResponseDTO() { Flag = true, Message = "Leave allocated successfully." };
        }

        private decimal CalculateLeaveDays(ApplicationUser employee, LeaveTypeConfig leaveConfig, int year)
        {
            if (leaveConfig.LeaveType.AllowGenderType != ApplicationConstants.Zero &&
                leaveConfig.LeaveType.AllowGenderType != (int)employee.Gender)
            {
                return 0;
            }
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
                existingAllocation.UpdateDate = DateTime.Now;
                existingAllocation.UpdatedByUserId = currentUserService.UserId;
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
                    RemainingLeaveCount = leaveDays,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                    IsActive = true
                };
                staffAppDbContext.EmployeeLeaveAllocations.Add(newAllocation);
            }

            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);
            return true;
        }


    }
}
