using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.EmploymentLeave;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Leave;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveAllocationService(IStaffAppDbContext staffAppDbContext, ICompanyYearService companyYearService, ICurrentUserService currentUserService) : ILeaveAllocationService
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

        public async Task<decimal> GetRemainingLeavesAsync(string employeeId, int leaveTypeId, DateTime startDate)
        {
            decimal remainingLeaveCount = 0;
            var leaveType = await staffAppDbContext.LeaveTypes
                .FirstOrDefaultAsync(lt => lt.Id == leaveTypeId);

            switch (leaveType.Name)
            {
                case LeaveTypeConstants.AnnualLeave:
                case LeaveTypeConstants.SickLeave:
                case LeaveTypeConstants.MaternityLeave:
                case LeaveTypeConstants.PaternityLeave:
                    {
                        var allocation = await staffAppDbContext.EmployeeLeaveAllocations
                            .FirstOrDefaultAsync(la =>
                                la.EmployeeId == employeeId &&
                                la.LeaveTypeId == leaveTypeId &&
                                la.CompanyYearId == DateTime.Now.Year);

                        remainingLeaveCount = allocation?.RemainingLeaveCount ?? 0;
                    }
                    break;
                case LeaveTypeConstants.ShortLeave:
                    {
                        DateTime startOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
                        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                        var leaveRequest = await staffAppDbContext.EmployeeLeaveRequests
                            .Where(lr =>
                                lr.EmployeeId == employeeId &&
                                lr.LeaveTypeId == leaveTypeId &&
                                lr.StartDate >= startOfMonth &&
                                lr.StartDate <= endOfMonth &&
                                (lr.CurrentStatus == Domain.Enum.LeaveStatus.Approved || lr.CurrentStatus == Domain.Enum.LeaveStatus.Pending))
                            .ToListAsync();

                        if (leaveRequest.Count >= 2)
                        {
                            return 0;
                        }

                        return ApplicationConstants.AllowMaximumShortLeavesPerMonth - leaveRequest.Count;
                    }
                    break;
                case LeaveTypeConstants.NoPayLeave:
                    {
                        remainingLeaveCount = 365 * 3;
                    }
                    break;
            }


            return remainingLeaveCount;
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

        public async Task<LeaveDashboardDTO> GetMyLeaveDashboardData()
        {
            var leaveDashboard = new LeaveDashboardDTO();

            var currentUserId = currentUserService.UserId;

            var loggedInUser = await staffAppDbContext.ApplicationUsers.FindAsync(currentUserId);

            var currentAcademicYear = await companyYearService.GetCurrentYear();

            if (currentAcademicYear is not null)
            {
                var leaveAllocations = await staffAppDbContext.EmployeeLeaveAllocations
                    .Where(x => x.EmployeeId == currentUserId && x.CompanyYearId == currentAcademicYear.Id)
                    .ToListAsync();

                foreach (var leaveAllocation in leaveAllocations)
                {
                    if (leaveAllocation.LeaveType.AllowGenderType == ApplicationConstants.One)
                    {
                        if (loggedInUser.Gender != (Gender)ApplicationConstants.One)
                            continue;
                    }

                    if (leaveAllocation.LeaveType.AllowGenderType == ApplicationConstants.Two)
                    {
                        if (loggedInUser.Gender != (Gender)ApplicationConstants.Two)
                            continue;
                    }

                    if (leaveAllocation.LeaveType != null)
                    {

                        var approvedLeaveRequest = leaveAllocation.LeaveType.EmployeeLeaveRequests
                            .Where(x => x.EmployeeId == currentUserId && x.CurrentStatus == LeaveStatus.Approved)
                            .ToList();

                        var pendingLeaveRequest = leaveAllocation.LeaveType.EmployeeLeaveRequests
                            .Where(x => x.EmployeeId == currentUserId && x.CurrentStatus == LeaveStatus.Pending)
                            .ToList();

                        var rejectedLeaveRequest = leaveAllocation.LeaveType.EmployeeLeaveRequests
                            .Where(x => x.EmployeeId == currentUserId && x.CurrentStatus == LeaveStatus.Rejected)
                            .ToList();

                        leaveDashboard.LeaveTypeSummaries.Add(new MyLeaveTypeSummaryDTO()
                        {
                            LeaveTypeId = leaveAllocation.LeaveType.Id,
                            LeaveTypeName = leaveAllocation.LeaveType.Name,
                            TotalLeaveDays = leaveAllocation.AllocatedLeaveCount,
                            RemainingLeaveDays = leaveAllocation.RemainingLeaveCount,
                            ApprovedLeaveDays = approvedLeaveRequest.Count,
                            PendingLeaveDays = pendingLeaveRequest.Count,
                            RejectedLeaveDays = rejectedLeaveRequest.Count
                        });
                    }
                }
            }

            return leaveDashboard;
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
