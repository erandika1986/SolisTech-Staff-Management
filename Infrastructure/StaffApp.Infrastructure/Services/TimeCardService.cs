using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.TimeCard;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class TimeCardService(IStaffAppDbContext context, ICurrentUserService currentUserService, IUserService userService) : ITimeCardService
    {
        public async Task<GeneralResponseDTO> ApproveTimeCard(int timeCardId, int timeCardEntryId, string comment)
        {
            try
            {
                var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Id == timeCardId);

                if (timeCard == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Time card not found."
                    };
                }

                var timeCardEntry = timeCard.TimeCardEntries.FirstOrDefault(x => x.Id == timeCardEntryId);
                timeCardEntry.ManagerComment = comment;
                timeCardEntry.Status = Domain.Enum.TimeCardEntryStatus.Approved;
                await context.SaveChangesAsync(CancellationToken.None);

                context.TimeCardEntries.Update(timeCardEntry);

                if (timeCard.TimeCardEntries.Count() == ApplicationConstants.One ||
                    timeCard.TimeCardEntries.Count(x => x.Status == Domain.Enum.TimeCardEntryStatus.Approved) == timeCard.TimeCardEntries.Count)
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.FullyApproved;
                }
                else if (timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Rejected))
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.PartiallyApprovedAndRejected;
                }
                else if (timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Submitted)
                    && !(timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Rejected)))
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.PartiallyApproved;
                }

                context.TimeCards.Update(timeCard);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Time card approved successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"An error occurred while approving the time card: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteTimeCardAsync(int timeCardId)
        {
            try
            {
                var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Id == timeCardId);

                if (timeCard == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Time card not found."
                    };
                }

                foreach (var entry in timeCard.TimeCardEntries.ToList())
                {
                    context.TimeCardEntries.Remove(entry);
                }

                context.TimeCards.Remove(timeCard);

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Time card deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"An error occurred while deleting the time card: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponseDTO> GenerateTimeCardForSelectedMonth(int companyYear, int companyMonth)
        {
            try
            {
                var activeEmployees = await context.ApplicationUsers
                    .Where(x => x.IsActive)
                    .ToListAsync();

                foreach (var employee in activeEmployees)
                {
                    var existingTimeCards = await context.TimeCards
                            .Where(x => x.EmployeeID == employee.Id && x.Date.Year == companyYear && x.Date.Month == companyMonth)
                            .ToListAsync(CancellationToken.None);
                    if (existingTimeCards.Count == 0)
                    {
                        DateTime startDate = new DateTime(companyYear, companyMonth, 1);
                        int daysInMonth = DateTime.DaysInMonth(companyYear, companyMonth);

                        for (int day = 0; day < daysInMonth; day++)
                        {
                            DateTime currentDate = startDate.AddDays(day);

                            var approvedLeaveRequest = context.EmployeeLeaveRequests
                                .FirstOrDefault(x => x.EmployeeId == employee.Id && x.StartDate <= currentDate && x.EndDate >= currentDate && x.CurrentStatus == Domain.Enum.LeaveStatus.Approved && x.LeaveDuration == Domain.Enum.LeaveDuration.FullDay);

                            var newTimeCard = new TimeCard
                            {
                                EmployeeID = employee.Id,
                                Date = currentDate,
                                Status = approvedLeaveRequest is null ? Domain.Enum.TimeCardStatus.Pending : Domain.Enum.TimeCardStatus.OnLeave
                            };
                            context.TimeCards.Add(newTimeCard);
                        }
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Time cards have been generated for selected month."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"An error occurred while generating time cards: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponseDTO> SetTimeCardOnHoliday(string employeeId, DateTime startDate, DateTime endDate)
        {
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var timeCard = context.TimeCards
                    .FirstOrDefault(x => x.EmployeeID == employeeId && x.Date == date);

                if (timeCard != null)
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.OnLeave;
                    context.TimeCards.Update(timeCard);

                    await context.SaveChangesAsync(CancellationToken.None);
                }
            }
            return new GeneralResponseDTO
            {
                Flag = true,
                Message = $"Time cards have been set to On Leave."
            };
        }

        public async Task<PaginatedResultDTO<BasicTimeCardDTO>> GetAllTimeCardAsync(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate)
        {
            var query = context.TimeCards.Where(x => x.Date >= fromDate && x.Date <= toDate && x.EmployeeID == currentUserService.UserId);

            int totalCount = await query.CountAsync();

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(timeCard => new BasicTimeCardDTO
                        {
                            Id = timeCard.Id,
                            EmployeeName = timeCard.Employee.FullName,
                            Date = timeCard.Date,
                            DateByString = timeCard.Date.ToString("dd/MM/yyyy"),
                            StatusName = EnumHelper.GetEnumDescription(timeCard.Status),
                            Status = timeCard.Status,
                            NumberOfProjects = timeCard.TimeCardEntries.Count,
                            TotalHours = timeCard.TimeCardEntries.Sum(x => x.HoursWorked)
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<BasicTimeCardDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<PaginatedResultDTO<BasicManagerTimeCardDTO>> GetMyEmployeeTimeCardsForSelectedDateAsync(int pageNumber, int pageSize, DateTime timeCardDate)
        {
            var assignedRoles = await userService
                .GetLoggedInUserAssignedRoles(currentUserService.UserId);

            var query = context.TimeCards
                .Where(x => x.Date == timeCardDate)
                .SelectMany(e => e.TimeCardEntries);

            if (!assignedRoles.Any(x => x == RoleConstants.Admin) &&
                !assignedRoles.Any(x => x == RoleConstants.Director) &&
                (assignedRoles.Any(x => x == RoleConstants.Manager) || assignedRoles.Any(x => x == RoleConstants.TeamLead)))
            {
                query = query
                    .Where(x => x.Project.ManagerId == currentUserService.UserId);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(tce => new BasicManagerTimeCardDTO
                        {
                            TimeCardId = tce.TimeCard.Id,
                            TimeCardEntryId = tce.Id,
                            EmployeeName = tce.TimeCard.Employee.FullName,
                            Date = tce.TimeCard.Date,
                            DateByString = tce.TimeCard.Date.ToString("dd/MM/yyyy"),
                            StatusName = EnumHelper.GetEnumDescription(tce.Status),
                            Status = tce.Status,
                            ProjectName = tce.Project.Name,
                            TotalHours = tce.HoursWorked,
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<BasicManagerTimeCardDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<TimeCardDTO> GetTimeCardByDateAsync(DateTime date)
        {
            var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Date == date);

            if (timeCard == null)
            {
                return null;
            }

            return new TimeCardDTO()
            {
                Id = timeCard.Id,
                EmployeeID = timeCard.EmployeeID,
                EmployeeName = timeCard.Employee?.FullName ?? "Unknown",
                Date = timeCard.Date,
                Status = timeCard.Status,
                StatusName = timeCard.Status.ToString(),
                TimeCardEntries = timeCard.TimeCardEntries.Select(x => new TimeCardEntryDTO
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    HoursWorked = x.HoursWorked,
                    Notes = x.Notes,
                    ManagerComment = x.ManagerComment,
                }).ToList()
            };
        }

        public async Task<TimeCardDTO> GetTimeCardByIdAsync(int timeCardId)
        {
            var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Id == timeCardId);

            if (timeCard == null)
            {
                return null;
            }

            return new TimeCardDTO()
            {
                Id = timeCard.Id,
                EmployeeID = timeCard.EmployeeID,
                EmployeeName = timeCard.Employee?.FullName ?? "Unknown",
                Date = timeCard.Date,
                DateByString = timeCard.Date.ToString("dd/MM/yyyy"),
                Status = timeCard.Status,
                StatusName = timeCard.Status.ToString(),
                TimeCardEntries = timeCard.TimeCardEntries.Select(x => new TimeCardEntryDTO
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    HoursWorked = x.HoursWorked,
                    Notes = x.Notes,
                    ManagerComment = x.ManagerComment,
                    ProjectName = x.Project?.Name ?? "Unknown Project",
                    TimeCardId = timeCard.Id,
                    Status = x.Status,
                    StatusName = EnumHelper.GetEnumDescription(x.Status)
                }).ToList()
            };
        }

        public async Task<GeneralResponseDTO> RejectTimeCard(int timeCardId, int timeCardEntryId, string comment)
        {
            try
            {
                var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Id == timeCardId);

                if (timeCard == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Time card not found."
                    };
                }

                var timeCardEntry = timeCard.TimeCardEntries.FirstOrDefault(x => x.Id == timeCardEntryId);
                timeCardEntry.ManagerComment = comment;
                timeCardEntry.Status = Domain.Enum.TimeCardEntryStatus.Rejected;
                await context.SaveChangesAsync(CancellationToken.None);

                context.TimeCardEntries.Update(timeCardEntry);

                if (timeCard.TimeCardEntries.Count() == ApplicationConstants.One ||
                    timeCard.TimeCardEntries.Count(x => x.Status == Domain.Enum.TimeCardEntryStatus.Rejected) == timeCard.TimeCardEntries.Count)
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.FullyRejected;
                }
                else if (timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Approved))
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.PartiallyApprovedAndRejected;
                }
                else if (timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Submitted)
                    && !(timeCard.TimeCardEntries.Any(x => x.Status == Domain.Enum.TimeCardEntryStatus.Approved)))
                {
                    timeCard.Status = Domain.Enum.TimeCardStatus.PartiallyRejected;
                }

                context.TimeCards.Update(timeCard);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Time card rejected successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"An error occurred while approving the time card: {ex.Message}"
                };
            }
        }

        public async Task<GeneralResponseDTO> SaveTimeCardAsync(TimeCardDTO timeCardDTO)
        {
            try
            {
                var timeCard = await context.TimeCards.FirstOrDefaultAsync(x => x.Id == timeCardDTO.Id);

                if (timeCard == null)
                {
                    timeCard = new TimeCard
                    {
                        EmployeeID = currentUserService.UserId,
                        Date = timeCardDTO.Date,
                        Status = Domain.Enum.TimeCardStatus.Submitted
                    };

                    foreach (var item in timeCardDTO.TimeCardEntries)
                    {
                        timeCard.TimeCardEntries.Add(new TimeCardEntry
                        {
                            ProjectId = item.ProjectId,
                            HoursWorked = item.HoursWorked,
                            Notes = item.Notes,
                            Status = Domain.Enum.TimeCardEntryStatus.Submitted
                        });
                    }

                    context.TimeCards.Add(timeCard);
                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO
                    {
                        Flag = true,
                        Message = "New time card added successfully."
                    };
                }
                else
                {
                    timeCard.Date = timeCardDTO.Date;
                    timeCard.Status = Domain.Enum.TimeCardStatus.Submitted;

                    var newlyAddedEntries = timeCardDTO.TimeCardEntries
                         .Where(x => x.Id == 0)
                         .ToList();

                    var deletedEntries = (from d in timeCard.TimeCardEntries
                                          where !timeCardDTO.TimeCardEntries.Any(x => x.Id == d.Id)
                                          select d).ToList();

                    var updatedEntries = timeCardDTO.TimeCardEntries
                        .Where(x => x.Id > 0 && x.IsModified == true)
                        .ToList();

                    foreach (var item in newlyAddedEntries)
                    {
                        timeCard.TimeCardEntries.Add(new TimeCardEntry
                        {
                            ProjectId = item.ProjectId,
                            HoursWorked = item.HoursWorked,
                            Notes = item.Notes,
                            Status = Domain.Enum.TimeCardEntryStatus.Submitted
                        });
                    }

                    context.TimeCards.Update(timeCard);
                    await context.SaveChangesAsync(CancellationToken.None);


                    foreach (var entry in updatedEntries)
                    {
                        var existingEntry = timeCard.TimeCardEntries.FirstOrDefault(x => x.Id == entry.Id);
                        if (existingEntry != null)
                        {
                            existingEntry.ProjectId = entry.ProjectId;
                            existingEntry.HoursWorked = entry.HoursWorked;
                            existingEntry.Notes = entry.Notes;
                            existingEntry.Status = Domain.Enum.TimeCardEntryStatus.Submitted;

                            context.TimeCardEntries.Update(existingEntry);
                        }
                    }

                    await context.SaveChangesAsync(CancellationToken.None);


                    foreach (var entry in deletedEntries)
                    {
                        context.TimeCardEntries.Remove(entry);

                    }

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO
                    {
                        Flag = true,
                        Message = "Time card updated successfully."
                    };
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = $"An error occurred while saving the time card: {ex.Message}"
                };
            }

        }

        public async Task<PaginatedResultDTO<MonthlyTimeCardDTO>> GetTimeCardsByMonthAsync(int pageNumber, int pageSize, int CompanyYearId, int MonthId)
        {
            var query = context.TimeCards.Where(x => x.Date.Year == CompanyYearId && x.Date.Month == MonthId);

            int totalCount = await query.CountAsync();

            var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(tce => new MonthlyTimeCardDTO
            {
                TimeCardId = tce.Id,
                CompanyYear = tce.Date.Year,
                Month = tce.Date.Month,
                EmployeeName = tce.Employee.FullName,
                Day = tce.Date.Day,
                Status = tce.Status,
                StatusName = EnumHelper.GetEnumDescription(tce.Status)
            })
            .ToListAsync();

            var newResult = new PaginatedResultDTO<MonthlyTimeCardDTO>
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
