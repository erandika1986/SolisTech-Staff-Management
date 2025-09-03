using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.DisciplinaryAction;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class UserDisciplinaryService(
        IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        IAzureBlobService azureBlobService,
        IConfiguration configuration,
        ILogger<IUserDisciplinaryService> logger) : IUserDisciplinaryService
    {
        public async Task<GeneralResponseDTO> AddAsync(DisciplinaryActionDTO action)
        {
            try
            {
                var disciplinaryAction = new UserDisciplinaryAction
                {
                    UserId = action.UserId,
                    ActionType = action.ActionType,
                    ActionDate = action.ActionDate.Value,
                    EffectiveUntil = action.EffectiveUntil,
                    Reason = action.Reason,
                    SeverityLevel = action.SeverityLevel,
                    Remarks = action.Remarks,
                    CreatedByUserId = currentUserService.UserId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.UtcNow,
                    IsActive = true
                };

                context.UserDisciplinaryActions.Add(disciplinaryAction);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Disciplinary action added successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding disciplinary action for user {UserId}", action.UserId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while adding the disciplinary action."
                };
            }
        }

        public async Task<GeneralResponseDTO> DeleteAsync(int id)
        {
            try
            {
                var entity = await context.UserDisciplinaryActions.FindAsync(id);
                if (entity == null)
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Disciplinary action not found."
                    };

                entity.IsActive = false;
                entity.UpdatedByUserId = currentUserService.UserId;
                entity.UpdateDate = DateTime.UtcNow;
                context.UserDisciplinaryActions.Update(entity);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Disciplinary action deleted successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting disciplinary action with ID {Id}", id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while deleting the disciplinary action."
                };
            }

        }

        public async Task<PaginatedResultDTO<DisciplinaryActionDTO>> GetAllAsync(int pageNumber, int pageSize, string searchText)
        {
            var query = context.UserDisciplinaryActions
                .Include(u => u.User) // optional if you want user details
                .Where(uda => uda.IsActive)
                .OrderByDescending(x => x.ActionDate);

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(uda =>
                    uda.Reason.Contains(searchText) ||
                    uda.Remarks.Contains(searchText) ||
                    uda.User.FullName.Contains(searchText)// Assuming User has a FullName property
                ).OrderByDescending(x => x.ActionDate); ;
            }

            var newResult = await GetPaginatedResultAsync(query, pageNumber, pageSize);

            return newResult;
        }

        public async Task<DisciplinaryActionDTO> GetByIdAsync(int id)
        {
            var disciplinaryAction = await context.UserDisciplinaryActions
                .Include(u => u.User) // optional if you want user details
                .FirstOrDefaultAsync(x => x.Id == id);

            var response = disciplinaryAction == null ? null : new DisciplinaryActionDTO
            {
                Id = disciplinaryAction.Id,
                UserId = disciplinaryAction.UserId,
                ActionType = disciplinaryAction.ActionType,
                ActionDate = disciplinaryAction.ActionDate,
                EffectiveUntil = disciplinaryAction.EffectiveUntil,
                Reason = disciplinaryAction.Reason,
                SeverityLevel = disciplinaryAction.SeverityLevel,
                Remarks = disciplinaryAction.Remarks
            };

            return response;
        }

        public async Task<PaginatedResultDTO<DisciplinaryActionDTO>> GetByUserIdAsync(int pageNumber, int pageSize, string userId)
        {
            var query = context.UserDisciplinaryActions
                .Include(u => u.User) // optional if you want user details
                .Where(uda => uda.IsActive && uda.UserId == userId)
                .OrderByDescending(x => x.ActionDate);

            var newResult = await GetPaginatedResultAsync(query, pageNumber, pageSize);

            return newResult;
        }

        public async Task<GeneralResponseDTO> UpdateAsync(DisciplinaryActionDTO action)
        {
            try
            {
                var entity = await context.UserDisciplinaryActions.FindAsync(action.Id);
                if (entity == null || !entity.IsActive)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "Disciplinary action not found."
                    };
                }

                entity.ActionType = action.ActionType;
                entity.ActionDate = action.ActionDate.Value;
                entity.EffectiveUntil = action.EffectiveUntil;
                entity.Reason = action.Reason;
                entity.SeverityLevel = action.SeverityLevel;
                entity.Remarks = action.Remarks;
                entity.UpdatedByUserId = currentUserService.UserId;
                entity.UpdateDate = DateTime.UtcNow;

                context.UserDisciplinaryActions.Update(entity);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "Disciplinary action updated successfully."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating disciplinary action with ID {Id}", action.Id);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while updating the disciplinary action."
                };
            }
        }

        public DisciplinaryActionMasterDTO GetDisciplinaryActionMasterData()
        {
            var actionTypes = EnumHelper.GetDropDownList<Domain.Enum.DisciplinaryActionType>();
            var severityLevels = EnumHelper.GetDropDownList<Domain.Enum.SeverityLevel>();

            return new DisciplinaryActionMasterDTO
            {
                ActionTypes = actionTypes,
                SeverityLevels = severityLevels
            };
        }

        private async Task<PaginatedResultDTO<DisciplinaryActionDTO>> GetPaginatedResultAsync(IOrderedQueryable<UserDisciplinaryAction> query, int pageNumber, int pageSize)
        {
            var totalRecords = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(uda => new DisciplinaryActionDTO
                {
                    Id = uda.Id,
                    UserId = uda.UserId,
                    ActionType = uda.ActionType,
                    ActionDate = uda.ActionDate,
                    EffectiveUntil = uda.EffectiveUntil,
                    Reason = uda.Reason,
                    SeverityLevel = uda.SeverityLevel,
                    Remarks = uda.Remarks
                })
                .ToListAsync();

            return new PaginatedResultDTO<DisciplinaryActionDTO>
            {
                Items = items,
                TotalItems = totalRecords,
                Page = pageNumber,
                PageSize = pageSize,
                IsReadOnly = true
            };
        }

    }
}
