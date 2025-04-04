using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.EmploymentLeave;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class LeaveRequestService(
        IStaffAppDbContext staffAppDbContext,
        IUserService userService,
        ILeaveAllocationService leaveAllocationService,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration) : ILeaveRequestService
    {
        public async Task<GeneralResponseDTO> ApproveLeaveRequestAsync(
            int leaveRequestId, string comment)
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
            leaveRequest.UpdateDate = DateTime.Now;
            leaveRequest.UpdatedByUserId = currentUserService.UserId;

            leaveRequest.EmployeeLeaveRequestComments.Add(new EmployeeLeaveRequestComment()
            {
                Comment = comment,
                CreatedByUserId = currentUserService.UserId,
                CreatedDate = DateTime.Now,
                Status = Domain.Enum.LeaveStatus.Approved
            });

            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave has been approved successfully." };
        }

        public async Task<GeneralResponseDTO> CreateLeaveRequestAsync(
            EmployeeLeaveRequestDTO leaveRequest)
        {
            // Validate leave request
            if (!await ValidateLeaveRequest(leaveRequest))
                return new GeneralResponseDTO() { Flag = false, Message = "Leave validation failed." };
            var employeeLeaveRequest = leaveRequest.Adapt<EmployeeLeaveRequest>();

            employeeLeaveRequest.AssignReportingManagerId = leaveRequest.SelectedReportingManager.Id;
            employeeLeaveRequest.LeaveTypeId = leaveRequest.SelectedLeaveType.Id;
            employeeLeaveRequest.LeaveDuration = (LeaveDuration)leaveRequest.SelectedLeaveDuration.Id;
            employeeLeaveRequest.CompanyYearId = leaveRequest.SelectedCompanyYear.Id;
            employeeLeaveRequest.CreatedDate = DateTime.Now;
            employeeLeaveRequest.CreatedByUserId = currentUserService.UserId;
            employeeLeaveRequest.UpdateDate = DateTime.Now;
            employeeLeaveRequest.UpdatedByUserId = currentUserService.UserId;
            employeeLeaveRequest.IsActive = true;

            // Calculate number of working days
            employeeLeaveRequest.NumberOfDays = CalculateWorkingDays(leaveRequest.StartDate.Value, leaveRequest.EndDate.Value);
            employeeLeaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Pending;

            employeeLeaveRequest.EmployeeLeaveRequestComments.Add(new EmployeeLeaveRequestComment()
            {
                Comment = $"Initiate leave request from employee id : {currentUserService.UserId}",
                CreatedByUserId = currentUserService.UserId,
                CreatedDate = DateTime.Now,
                Status = Domain.Enum.LeaveStatus.Approved
            });

            switch ((LeaveDuration)leaveRequest.SelectedLeaveDuration.Id)
            {
                case LeaveDuration.HalfDay:
                    {
                        employeeLeaveRequest.HalfDaySessionType = (HalfDaySessionType)leaveRequest.SelectedHalfDaySessionType.Id;
                        if ((HalfDaySessionType)leaveRequest.SelectedHalfDaySessionType.Id == HalfDaySessionType.Morning)
                        {
                            employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.HalfDayMorningStartHour, ApplicationConstants.HalfDayMorningStartMin, 0);
                            employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.HalfDayMorningEndHour, ApplicationConstants.HalfDayMorningEndMin, 0);
                        }
                        else
                        {
                            employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.HalfDayAfternoonStartHour, ApplicationConstants.HalfDayAfternoonStartMin, 0);
                            employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.HalfDayAfternoonEndHour, ApplicationConstants.HalfDayAfternoonEndMin, 0);
                        }
                    }
                    break;
                case LeaveDuration.ShortLeave:
                    {
                        employeeLeaveRequest.ShortLeaveSessionType = (ShortLeaveSessionType)leaveRequest.SelectedShortLeaveSessionType.Id;
                        if ((ShortLeaveSessionType)leaveRequest.SelectedShortLeaveSessionType.Id == ShortLeaveSessionType.Morning)
                        {
                            employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.ShortLeaveMorningStartHour, ApplicationConstants.ShortLeaveMorningStartMin, 0);
                            employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.ShortLeaveMorningEndHour, ApplicationConstants.ShortLeaveMorningEndMin, 0);
                        }
                        else
                        {
                            employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.ShortLeaveAfternoonStartHour, ApplicationConstants.ShortLeaveAfternoonStartMin, 0);
                            employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.ShortLeaveAfternoonEndHour, ApplicationConstants.ShortLeaveAfternoonEndMin, 0);
                        }
                    }
                    break;
                default:
                    {

                    }
                    break;
            }


            employeeLeaveRequest = await UploadFiles(leaveRequest, employeeLeaveRequest);

            staffAppDbContext.EmployeeLeaveRequests.Add(employeeLeaveRequest);
            await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO() { Flag = true, Message = "Leave request has been created successfully." };
        }

        public async Task<GeneralResponseDTO> UpdateLeaveRequestAsync(
            EmployeeLeaveRequestDTO leaveRequest)
        {
            try
            {
                if (!await ValidateLeaveRequest(leaveRequest))
                    return new GeneralResponseDTO() { Flag = false, Message = "Leave validation failed." };
                var employeeLeaveRequest = await staffAppDbContext.EmployeeLeaveRequests.FindAsync(leaveRequest.Id);

                employeeLeaveRequest.AssignReportingManagerId = leaveRequest.SelectedReportingManager.Id;
                employeeLeaveRequest.LeaveTypeId = leaveRequest.SelectedLeaveType.Id;
                employeeLeaveRequest.StartDate = leaveRequest.StartDate.Value;
                employeeLeaveRequest.EndDate = leaveRequest.EndDate.Value;
                employeeLeaveRequest.LeaveDuration = (LeaveDuration)leaveRequest.SelectedLeaveDuration.Id;
                employeeLeaveRequest.CompanyYearId = leaveRequest.SelectedCompanyYear.Id;
                employeeLeaveRequest.UpdateDate = DateTime.Now;
                employeeLeaveRequest.UpdatedByUserId = currentUserService.UserId;

                // Calculate number of working days
                employeeLeaveRequest.NumberOfDays = CalculateWorkingDays(leaveRequest.StartDate.Value, leaveRequest.EndDate.Value);
                employeeLeaveRequest.CurrentStatus = Domain.Enum.LeaveStatus.Pending;

                switch ((LeaveDuration)leaveRequest.SelectedLeaveDuration.Id)
                {
                    case LeaveDuration.HalfDay:
                        {
                            employeeLeaveRequest.ShortLeaveSessionType = (ShortLeaveSessionType?)null;
                            employeeLeaveRequest.HalfDaySessionType = (HalfDaySessionType)leaveRequest.SelectedHalfDaySessionType.Id;
                            if ((HalfDaySessionType)leaveRequest.SelectedHalfDaySessionType.Id == HalfDaySessionType.Morning)
                            {
                                employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.HalfDayMorningStartHour, ApplicationConstants.HalfDayMorningStartMin, 0);
                                employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.HalfDayMorningEndHour, ApplicationConstants.HalfDayMorningEndMin, 0);
                            }
                            else
                            {
                                employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.HalfDayAfternoonStartHour, ApplicationConstants.HalfDayAfternoonStartMin, 0);
                                employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.HalfDayAfternoonEndHour, ApplicationConstants.HalfDayAfternoonEndMin, 0);
                            }
                        }
                        break;
                    case LeaveDuration.ShortLeave:
                        {
                            employeeLeaveRequest.ShortLeaveSessionType = (ShortLeaveSessionType)leaveRequest.SelectedShortLeaveSessionType.Id;
                            employeeLeaveRequest.HalfDaySessionType = (HalfDaySessionType?)null;
                            if ((ShortLeaveSessionType)leaveRequest.SelectedShortLeaveSessionType.Id == ShortLeaveSessionType.Morning)
                            {
                                employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.ShortLeaveMorningStartHour, ApplicationConstants.ShortLeaveMorningStartMin, 0);
                                employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.ShortLeaveMorningEndHour, ApplicationConstants.ShortLeaveMorningEndMin, 0);
                            }
                            else
                            {
                                employeeLeaveRequest.StartTime = new TimeSpan(ApplicationConstants.ShortLeaveAfternoonStartHour, ApplicationConstants.ShortLeaveAfternoonStartMin, 0);
                                employeeLeaveRequest.EndTime = new TimeSpan(ApplicationConstants.ShortLeaveAfternoonEndHour, ApplicationConstants.ShortLeaveAfternoonEndMin, 0);
                            }
                        }
                        break;
                    default:
                        {
                            employeeLeaveRequest.HalfDaySessionType = (HalfDaySessionType?)null;
                            employeeLeaveRequest.ShortLeaveSessionType = (ShortLeaveSessionType?)null;
                            employeeLeaveRequest.StartTime = (TimeSpan?)null;
                            employeeLeaveRequest.EndTime = (TimeSpan?)null;
                        }
                        break;
                }

                employeeLeaveRequest = await UploadFiles(leaveRequest, employeeLeaveRequest);

                staffAppDbContext.EmployeeLeaveRequests.Update(employeeLeaveRequest);
                await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Leave request has been updated successfully." };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };
            }

        }

        public async Task<GeneralResponseDTO> RejectLeaveRequestAsync(
            int leaveRequestId, string comment)
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

        public async Task<PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>> GetMyLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus,
            DateTime? startDate, DateTime? endDate)
        {

            var fromDate = startDate.Value.Date;
            var toDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var query = staffAppDbContext.EmployeeLeaveRequests
                .Where(x => x.CompanyYearId == companyYear && x.EmployeeId == currentUserService.UserId && x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                .OrderBy(x => x.StartDate);

            if (leaveTypeId != ApplicationConstants.Zero)
                query = query.Where(x => x.LeaveTypeId == leaveTypeId).OrderBy(x => x.CreatedDate);

            if (leaveStatus != ApplicationConstants.Zero)
                query = query.Where(x => x.CurrentStatus == (LeaveStatus)leaveStatus).OrderBy(x => x.CreatedDate);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BasicEmployeeLeaveRequestDTO()
                {
                    Id = x.Id,
                    SelectedLeaveType = new DropDownDTO() { Id = x.LeaveTypeId },
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.FullName,
                    LeaveType = x.LeaveType.Name,
                    StartDate = x.LeaveDuration == LeaveDuration.FullDay ? String.Format("{0:MM/dd/yyyy}", x.StartDate) : String.Format("{0:MM/dd/yyyy}", x.StartDate),
                    EndDate = x.LeaveDuration == LeaveDuration.FullDay ? String.Format("{0:MM/dd/yyyy}", x.EndDate) : String.Format("{0:MM/dd/yyyy}", x.EndDate),
                    NumberOfDays = x.NumberOfDays,
                    CurrentStatus = EnumHelper.GetEnumDescription((LeaveStatus)x.CurrentStatus),
                    LeaveDuration = EnumHelper.GetEnumDescription(x.LeaveDuration),

                }).ToListAsync();
            var newResult = new PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<EmployeeLeaveRequestDTO> GetLeaveRequestById(
            int leaveRequestId)
        {
            var leaveRequest = await staffAppDbContext.EmployeeLeaveRequests.FindAsync(leaveRequestId);
            if (leaveRequest == null)
            {
                var currentUser = await staffAppDbContext.ApplicationUsers.FindAsync(currentUserService.UserId);
                return new EmployeeLeaveRequestDTO()
                {
                    EmployeeId = currentUser.Id,
                    EmployeeName = currentUser.FullName,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    SelectedLeaveDuration = new DropDownDTO() { Id = (int)LeaveDuration.FullDay }
                };
            }

            return new EmployeeLeaveRequestDTO()
            {
                Id = leaveRequest.Id,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = leaveRequest.Employee.FullName,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.NumberOfDays,
                CurrentStatus = (LeaveStatus)leaveRequest.CurrentStatus,
                SelectedLeaveType = new DropDownDTO() { Id = leaveRequest.LeaveTypeId },
                SelectedLeaveDuration = new DropDownDTO() { Id = (int)leaveRequest.LeaveDuration },
                SelectedReportingManager = new UserDropDownDTO() { Id = leaveRequest.AssignReportingManagerId },
                SelectedCompanyYear = new DropDownDTO() { Id = leaveRequest.CompanyYearId },
                Reason = leaveRequest.Reason,
                SelectedHalfDaySessionType = leaveRequest.HalfDaySessionType.HasValue ? new DropDownDTO() { Id = (int)leaveRequest.HalfDaySessionType.Value } : null,
                SelectedShortLeaveSessionType = leaveRequest.ShortLeaveSessionType.HasValue ? new DropDownDTO() { Id = (int)leaveRequest.ShortLeaveSessionType.Value } : null,
                SavedSupportFiles = leaveRequest
                                        .EmployeeLeaveRequestSupportFiles
                                        .Select(x => new EmployeeLeaveRequestSupportFileDTO()
                                        {
                                            Id = x.Id,
                                            OriginalFileName = x.OriginalFileName,
                                            SavedFileName = x.SavedFileName,
                                            SaveFileURL = x.SaveFileURL,
                                        }).ToList()
            };
        }

        public async Task<GeneralResponseDTO> RemoveSavedSupportFile(
            EmployeeLeaveRequestSupportFileDTO file)
        {
            try
            {
                await azureBlobService.DeleteFileAsync(file.SavedFileName, ApplicationConstants.AzureBlobStorageName);
                var recordToRemove = await staffAppDbContext.EmployeeLeaveRequestSupportFiles.FindAsync(file.Id);
                staffAppDbContext.EmployeeLeaveRequestSupportFiles.Remove(recordToRemove);

                await staffAppDbContext.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO() { Flag = true, Message = "Saved supporting file successfully deleted from the system" };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO() { Flag = false, Message = ex.Message };

            }
        }

        private async Task<bool> ValidateLeaveRequest(
            EmployeeLeaveRequestDTO leaveRequest)
        {
            // Check if employee has sufficient leave balance
            decimal remainingLeaves = await leaveAllocationService
                .GetRemainingLeavesAsync(leaveRequest.EmployeeId, leaveRequest.SelectedLeaveType.Id);

            // Calculate working days for the request
            decimal requestedDays = CalculateWorkingDays(leaveRequest.StartDate.Value, leaveRequest.EndDate.Value);

            // Validate leave balance
            if (requestedDays > remainingLeaves)
                return false;

            // Additional validations can be added
            return true;
        }

        private decimal CalculateWorkingDays(
            DateTime startDate,
            DateTime endDate)
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

        public async Task<List<UserDropDownDTO>> GetMyReportingManagers()
        {
            var response = new List<UserDropDownDTO>();

            var assignedDepartments = await staffAppDbContext.EmployeeDepartments.Where(x => x.IsActive && x.UserId == currentUserService.UserId).ToListAsync();

            foreach (var department in assignedDepartments)
            {
                if (!string.IsNullOrEmpty(department.Department.DepartmentHeadId))
                {
                    if (response.FirstOrDefault(x => x.Id == department.Department.DepartmentHeadId) == null)
                    {
                        response.Add(new UserDropDownDTO()
                        {
                            Id = department.Department.DepartmentHeadId,
                            Name = department.Department.DepartmentHead.FullName ?? string.Empty
                        });
                    }
                }
            }

            return response;
        }

        private async Task<EmployeeLeaveRequest> UploadFiles(
            EmployeeLeaveRequestDTO leaveRequestDTO,
            EmployeeLeaveRequest employeeLeaveRequest)
        {
            string leaveSupportDocumentPath = configuration["FileSavePaths:LeaveSupportDocumentPath"];
            if (!Directory.Exists(leaveSupportDocumentPath))
            {
                Directory.CreateDirectory(leaveSupportDocumentPath);
            }


            long maxFileSize = 10 * 1024 * 1024;

            for (int i = 0; i < leaveRequestDTO.Files.Count; i++)
            {
                var extension = Path.GetExtension(leaveRequestDTO.Files[i].Name);

                var fileName = $"{Path.GetFileNameWithoutExtension(leaveRequestDTO.Files[i].Name)}{extension}";
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(leaveSupportDocumentPath, uniqueFileName);

                await using var stream = leaveRequestDTO.Files[i].OpenReadStream(maxFileSize);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadedFileUrl = await azureBlobService
                    .UploadFileAsync(memoryStream, uniqueFileName, leaveRequestDTO.Files[i].ContentType, ApplicationConstants.AzureBlobStorageName);

                //await using var fileStream = new FileStream(filePath, FileMode.Create);
                //await stream.CopyToAsync(fileStream);

                employeeLeaveRequest.EmployeeLeaveRequestSupportFiles.Add(new EmployeeLeaveRequestSupportFile()
                {
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    OriginalFileName = fileName,
                    SavedFileName = uniqueFileName,
                    SaveFileURL = uploadedFileUrl,
                    UpdateDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                });
            }

            return employeeLeaveRequest;
        }

        public async Task<PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>> GetMyAssignedLeaveRequests(
            int pageNumber,
            int pageSize,
            int companyYear,
            int leaveTypeId,
            int leaveStatus,
            DateTime? startDate,
            DateTime? endDate)
        {
            var fromDate = startDate.Value.Date;
            var toDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);

            var assignedRoles = await userService
                .GetLoggedInUserAssignedRoles(currentUserService.UserId);

            var query = staffAppDbContext.EmployeeLeaveRequests
                .Where(x => x.CompanyYearId == companyYear && x.CreatedDate >= fromDate && x.CreatedDate <= toDate)
                .OrderBy(x => x.StartDate);

            if (!assignedRoles.Any(x => x == RoleConstants.Admin) &&
                !assignedRoles.Any(x => x == RoleConstants.Director) &&
                (assignedRoles.Any(x => x == RoleConstants.Manager) || assignedRoles.Any(x => x == RoleConstants.TeamLead)))
            {
                query = query
                    .Where(x => x.AssignReportingManagerId == currentUserService.UserId)
                    .OrderBy(x => x.StartDate); ;
            }

            if (leaveTypeId != ApplicationConstants.Zero)
                query = query
                    .Where(x => x.LeaveTypeId == leaveTypeId)
                    .OrderBy(x => x.CreatedDate);

            if (leaveStatus != ApplicationConstants.Zero)
                query = query
                    .Where(x => x.CurrentStatus == (LeaveStatus)leaveStatus)
                    .OrderBy(x => x.CreatedDate);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BasicEmployeeLeaveRequestDTO()
                {
                    Id = x.Id,
                    SelectedLeaveType = new DropDownDTO() { Id = x.LeaveTypeId },
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.Employee.FullName,
                    LeaveType = x.LeaveType.Name,
                    StartDate = x.LeaveDuration == LeaveDuration.FullDay ? String.Format("{0:MM/dd/yyyy}", x.StartDate) : String.Format("{0:MM/dd/yyyy}", x.StartDate),
                    EndDate = x.LeaveDuration == LeaveDuration.FullDay ? String.Format("{0:MM/dd/yyyy}", x.EndDate) : String.Format("{0:MM/dd/yyyy}", x.EndDate),
                    NumberOfDays = x.NumberOfDays,
                    CurrentStatus = EnumHelper.GetEnumDescription((LeaveStatus)x.CurrentStatus),
                    LeaveDuration = EnumHelper.GetEnumDescription(x.LeaveDuration),

                }).ToListAsync();

            var newResult = new PaginatedResultDTO<BasicEmployeeLeaveRequestDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public Task<GeneralResponseDTO> DeleteLeaveRequestAsync(int leaveRequestId, string comment)
        {
            throw new NotImplementedException();
        }
    }
}
