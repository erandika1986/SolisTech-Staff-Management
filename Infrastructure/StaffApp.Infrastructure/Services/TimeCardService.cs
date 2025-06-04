using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.TimeCard;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class TimeCardService(IStaffAppDbContext context, ICurrentUserService currentUserService) : ITimeCardService
    {
        public async Task<GeneralResponseDTO> ApproveTimeCard(int timeCardId, string comment)
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

                timeCard.Status = Domain.Enum.TimeCardStatus.Approved;
                timeCard.ManagerComment = comment;

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

        public async Task<PaginatedResultDTO<BasicTimeCardDTO>> GetAllTimeCardAsync(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate)
        {
            var query = context.TimeCards.Where(x => x.Date >= fromDate && x.Date <= toDate);

            int totalCount = await query.CountAsync();

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(timeCard => new BasicTimeCardDTO
                        {
                            Id = timeCard.Id,
                            EmployeeName = timeCard.Employee.FullName,
                            Date = timeCard.Date,
                            Notes = timeCard.Notes,
                            StatusName = EnumHelper.GetEnumDescription(timeCard.Status),
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
                Notes = timeCard.Notes,
                Status = timeCard.Status,
                StatusName = timeCard.Status.ToString(),
                ManagerComment = timeCard.ManagerComment,
                TimeCardEntries = timeCard.TimeCardEntries.Select(x => new TimeCardEntryDTO
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    HoursWorked = x.HoursWorked,
                    Notes = x.Notes
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
                Notes = timeCard.Notes,
                Status = timeCard.Status,
                StatusName = timeCard.Status.ToString(),
                ManagerComment = timeCard.ManagerComment,
                TimeCardEntries = timeCard.TimeCardEntries.Select(x => new TimeCardEntryDTO
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    HoursWorked = x.HoursWorked,
                    Notes = x.Notes
                }).ToList()
            };
        }

        public async Task<GeneralResponseDTO> RejectTimeCard(int timeCardId, string comment)
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

                timeCard.Status = Domain.Enum.TimeCardStatus.Rejected;
                timeCard.ManagerComment = comment;

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
                        Notes = timeCardDTO.Notes,
                        Status = Domain.Enum.TimeCardStatus.Pending,
                        ManagerComment = timeCardDTO.ManagerComment
                    };

                    foreach (var item in timeCardDTO.TimeCardEntries)
                    {
                        timeCard.TimeCardEntries.Add(new TimeCardEntry
                        {
                            ProjectId = item.ProjectId,
                            HoursWorked = item.HoursWorked,
                            Notes = item.Notes,
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
                    timeCard.Notes = timeCardDTO.Notes;

                    var newlyAddedEntries = timeCardDTO.TimeCardEntries
                         .Where(x => x.Id == 0)
                         .ToList();

                    foreach (var item in newlyAddedEntries)
                    {
                        timeCard.TimeCardEntries.Add(new TimeCardEntry
                        {
                            ProjectId = item.ProjectId,
                            HoursWorked = item.HoursWorked,
                            Notes = item.Notes,
                        });
                    }

                    context.TimeCards.Update(timeCard);
                    await context.SaveChangesAsync(CancellationToken.None);

                    var updatedEntries = timeCardDTO.TimeCardEntries
                        .Where(x => x.Id > 0)
                        .ToList();

                    foreach (var entry in updatedEntries)
                    {
                        var existingEntry = timeCard.TimeCardEntries.FirstOrDefault(x => x.Id == entry.Id);
                        if (existingEntry != null)
                        {
                            existingEntry.ProjectId = entry.ProjectId;
                            existingEntry.HoursWorked = entry.HoursWorked;
                            existingEntry.Notes = entry.Notes;

                            context.TimeCardEntries.Update(existingEntry);
                        }
                    }

                    await context.SaveChangesAsync(CancellationToken.None);

                    var deletedEntries = (from d in timeCard.TimeCardEntries
                                          where !timeCardDTO.TimeCardEntries.Any(x => x.Id == d.Id)
                                          select d).ToList();

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
    }
}
