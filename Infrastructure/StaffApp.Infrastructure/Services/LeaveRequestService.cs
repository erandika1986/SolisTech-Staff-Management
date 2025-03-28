using Mapster;
using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.EmploymentLeave;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveRequestService(
        IStaffAppDbContext staffAppDbContext,
        ILeaveAllocationService leaveAllocationService, ICurrentUserService currentUserService) : ILeaveRequestService
    {
        public async Task<GeneralResponseDTO> ApproveLeaveRequestAsync(int leaveRequestId)
        {
            var leaveRequest = await staffAppDbContext.EmployeeLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest == null)
                return new GeneralResponseDTO() { Flag = false, Message = "Leave request is not exists" };

            // Deduct leave from allocation
            var leaveAllocated = await leaveAllocationService
                .AllocateLeaveAsync(leaveRequest.EmployeeId, leaveRequest.LeaveType, leaveRequest.NumberOfDays);

            if (!leaveAllocated.Flag)
                return new GeneralResponseDTO() { Flag = false, Message = "Failed to approved the leave." };

            // Update leave request status
            leaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Approved;
            leaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Rejected;
            leaveRequest.UpdateDate = DateTime.Now;
            leaveRequest.UpdatedByUserId = currentUserService.UserId;

            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave has been approved successfully." };
        }

        public async Task<GeneralResponseDTO> CreateLeaveRequestAsync(EmployeeLeaveRequestDTO leaveRequest)
        {
            // Validate leave request
            if (!await ValidateLeaveRequest(leaveRequest))
                return new GeneralResponseDTO() { Flag = false, Message = "Leave validation failed." };
            var employeeLeaveRequest = leaveRequest.Adapt<EmployeeLeaveRequest>();
            employeeLeaveRequest.CreatedDate = DateTime.Now;
            employeeLeaveRequest.CreatedByUserId = currentUserService.UserId;
            employeeLeaveRequest.UpdateDate = DateTime.Now;
            employeeLeaveRequest.UpdatedByUserId = currentUserService.UserId;
            employeeLeaveRequest.IsActive = true;

            // Calculate number of working days
            employeeLeaveRequest.NumberOfDays = CalculateWorkingDays(DateTime.Parse(leaveRequest.StartDate), DateTime.Parse(leaveRequest.EndDate));
            employeeLeaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Pending;

            staffAppDbContext.EmployeeLeaveRequests.Add(employeeLeaveRequest);
            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave request has been created successfully." };
        }

        public async Task<GeneralResponseDTO> RejectLeaveRequestAsync(int leaveRequestId)
        {
            var leaveRequest = await staffAppDbContext.EmployeeLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == leaveRequestId);

            if (leaveRequest == null)
                return new GeneralResponseDTO() { Flag = false, Message = "Leave request is not exists" };

            // Update leave request status
            leaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Rejected;
            leaveRequest.UpdateDate = DateTime.Now;
            leaveRequest.UpdatedByUserId = currentUserService.UserId;
            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave request has been rejected." };
        }

        private async Task<bool> ValidateLeaveRequest(EmployeeLeaveRequestDTO leaveRequest)
        {
            // Check if employee has sufficient leave balance
            decimal remainingLeaves = await leaveAllocationService
                .GetRemainingLeavesAsync(leaveRequest.EmployeeId, leaveRequest.SelectedLeaveType.Id);

            // Calculate working days for the request
            decimal requestedDays = CalculateWorkingDays(DateTime.Parse(leaveRequest.StartDate), DateTime.Parse(leaveRequest.EndDate));

            // Validate leave balance
            if (requestedDays > remainingLeaves)
                return false;

            // Additional validations can be added
            return true;
        }

        private decimal CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            // Calculate only working days (excluding weekends)
            decimal workingDays = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday &&
                    date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

        public async Task<PaginatedResultDTO<EmployeeLeaveRequestDTO>> GetMyLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus)
        {
            var query = staffAppDbContext.EmployeeLeaveRequests.Where(x => x.CompanyYearId == companyYear).OrderByDescending(x => x.StartDate);

            if (leaveTypeId != ApplicationConstants.Zero)
                query = query.Where(x => x.LeaveTypeId == leaveTypeId).OrderByDescending(x => x.StartDate);

            if (leaveStatus != ApplicationConstants.Zero)
                query = query.Where(x => x.CurrentStatus == (LeaveStatus)leaveStatus).OrderByDescending(x => x.StartDate);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmployeeLeaveRequestDTO()
                {
                    Id = x.Id,
                    SelectedLeaveType = new DropDownDTO() { Id = x.LeaveTypeId },
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.FullName,
                    LeaveType = x.LeaveType.Name,
                    StartDate = x.StartDate.ToString(),
                    EndDate = x.EndDate.ToString(),
                    NumberOfDays = x.NumberOfDays,
                    CurrentStatus = (LeaveStatus)x.CurrentStatus,
                    LeaveDuration = x.LeaveDuration,

                }).ToListAsync();
            var newResult = new PaginatedResultDTO<EmployeeLeaveRequestDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }
    }
}
