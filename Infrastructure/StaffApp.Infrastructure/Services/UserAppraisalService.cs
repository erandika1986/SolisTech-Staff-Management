using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.UserAppraisal;
using StaffApp.Application.Services;

namespace StaffApp.Infrastructure.Services
{
    public class UserAppraisalService(IStaffAppDbContext context, ICurrentUserService currentUserService, IUserService userService) : IUserAppraisalService
    {
        public async Task<GeneralResponseDTO> GenerateUserRecordsForSelectedAppraisalPeriod(int appraisalPeriodId)
        {
            var response = new GeneralResponseDTO();

            try
            {
                var activeUsers = await userService.GetAllActiveUsersAsync();

                var existingRecords = await context.UserAppraisals.Where(ua => ua.AppraisalPeriodId == appraisalPeriodId).ToListAsync();

                var appraisalCriteria = await context.UserAppraisalCriterias.ToListAsync();

                foreach (var activeUser in activeUsers)
                {
                    if (!existingRecords.Any(er => er.UserId == activeUser.Id))
                    {
                        var newUserAppraisal = new Domain.Entity.UserAppraisal
                        {
                            AppraisalPeriodId = appraisalPeriodId,
                            UserId = activeUser.Id,
                            ReviewerId = null, // Assign a default reviewer if needed
                            OverallRating = 0,
                            Comments = string.Empty,
                            Status = Domain.Enum.AppraisalStatus.Pending,
                            CreatedByUserId = currentUserService.UserId,
                            CreatedDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            UpdatedByUserId = currentUserService.UserId,
                            IsActive = true
                        };

                        foreach (var criteria in appraisalCriteria)
                        {
                            var detail = new Domain.Entity.UserAppraisalDetail
                            {
                                CriteriaID = criteria.Id,
                                Rating = 0,
                                Comment = string.Empty,
                            };

                            newUserAppraisal.UserAppraisalDetails.Add(detail);
                        }

                        context.UserAppraisals.Add(newUserAppraisal);
                    }
                }

                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal records generated successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<UserAppraisalDTO> GetUserAppraisalDetailsById(int userAppraisalId)
        {
            var userAppraisal = await context.UserAppraisals
                .Include(ua => ua.UserAppraisalDetails)
                .FirstOrDefaultAsync(ua => ua.Id == userAppraisalId);

            var userAppraisalDTO = new UserAppraisalDTO()
            {
                AppraisalPeriodId = userAppraisal.AppraisalPeriodId,
                Comments = userAppraisal.Comments,
                Id = userAppraisal.Id,
                OverallRating = userAppraisal.OverallRating,
                ReviewerId = userAppraisal.ReviewerId,
                Status = userAppraisal.Status,
                UserId = userAppraisal.UserId,
                ReviewerFullName = !string.IsNullOrEmpty(userAppraisal.ReviewerId) ? (await userService.GetUserByIdAsync(userAppraisal.ReviewerId))?.FullName : string.Empty,
                UserFullName = (await userService.GetUserByIdAsync(userAppraisal.UserId))?.FullName,
            };

            foreach (var item in userAppraisal.UserAppraisalDetails)
            {
                userAppraisalDTO.UserAppraisalDetails.Add(new UserAppraisalDetailDTO()
                {
                    AppraisalID = item.AppraisalID,
                    Comment = item.Comment,
                    CriteriaID = item.CriteriaID,
                    Id = item.Id,
                    Rating = item.Rating
                });
            }

            return userAppraisalDTO;
        }

        public async Task<List<UserAppraisalSummaryDTO>> GetUsersAppraisalsByAppraisalPeriodId(int appraisalPeriodId)
        {
            var userAppraisals = await context.UserAppraisals
                .Where(ua => ua.AppraisalPeriodId == appraisalPeriodId)
                .ToListAsync();

            var userAppraisalSummaries = new List<UserAppraisalSummaryDTO>();

            foreach (var appraisal in userAppraisals)
            {
                var user = await userService.GetUserByIdAsync(appraisal.UserId);
                var reviewer = !string.IsNullOrEmpty(appraisal.ReviewerId) ? await userService.GetUserByIdAsync(appraisal.ReviewerId) : null;
                userAppraisalSummaries.Add(new UserAppraisalSummaryDTO
                {
                    AppraisalPeriodId = appraisal.AppraisalPeriodId,
                    Id = appraisal.Id,
                    OverallRating = appraisal.OverallRating,
                    Status = appraisal.Status,
                    UserFullName = user?.FullName,
                    ReviewerFullName = reviewer?.FullName,
                    Comments = appraisal.Comments,
                    ReviewerId = appraisal.ReviewerId,
                    UserId = appraisal.UserId
                });
            }

            return userAppraisalSummaries;
        }

        public async Task<GeneralResponseDTO> SaveUserAppraisal(UserAppraisalDTO userAppraisal)
        {
            try
            {
                var userAppraisalEntity = await context.UserAppraisals
                    .Include(ua => ua.UserAppraisalDetails)
                    .FirstOrDefaultAsync(ua => ua.Id == userAppraisal.Id);

                userAppraisalEntity.Comments = userAppraisal.Comments;
                userAppraisalEntity.ReviewerId = userAppraisal.ReviewerId;
                userAppraisalEntity.Status = Domain.Enum.AppraisalStatus.InReview;
                userAppraisalEntity.UpdateDate = DateTime.UtcNow;
                userAppraisalEntity.UpdatedByUserId = currentUserService.UserId;
                userAppraisalEntity.OverallRating = userAppraisal.UserAppraisalDetails.Average(x => x.Rating);

                foreach (var detailDTO in userAppraisal.UserAppraisalDetails)
                {
                    var detailEntity = userAppraisalEntity.UserAppraisalDetails
                        .FirstOrDefault(d => d.Id == detailDTO.Id);
                    if (detailEntity != null)
                    {
                        detailEntity.Rating = detailDTO.Rating;
                        detailEntity.Comment = detailDTO.Comment;
                    }
                }

                context.UserAppraisals.Update(userAppraisalEntity);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<GeneralResponseDTO> CompleteUserAppraisal(int userAppraisalId)
        {
            try
            {
                var userAppraisalEntity = await context.UserAppraisals
                    .FirstOrDefaultAsync(ua => ua.Id == userAppraisalId);
                if (userAppraisalEntity == null)
                {
                    return new GeneralResponseDTO
                    {
                        Flag = false,
                        Message = "User appraisal not found."
                    };
                }
                userAppraisalEntity.Status = Domain.Enum.AppraisalStatus.Completed;
                userAppraisalEntity.UpdateDate = DateTime.UtcNow;
                userAppraisalEntity.UpdatedByUserId = currentUserService.UserId;
                context.UserAppraisals.Update(userAppraisalEntity);
                await context.SaveChangesAsync(CancellationToken.None);
                return new GeneralResponseDTO
                {
                    Flag = true,
                    Message = "User appraisal completed successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Application.DTOs.Appraisal.EmployeeAppraisalDTO> GetEmployeeAppraisalById(int userAppraisalId)
        {
            var employeeAppraisal = new Application.DTOs.Appraisal.EmployeeAppraisalDTO();

            var userAppraisal = await context.UserAppraisals
                .FirstOrDefaultAsync(ua => ua.Id == userAppraisalId);

            var assignedRoles = await userService.GetLoggedInUserAssignedRoles(userAppraisal.UserId);

            var assignedDepartments = await context.EmployeeDepartments
                .Include(u => u.Department)
                .Where(u => u.UserId == userAppraisal.UserId)
                .Select(x => x.Department.Name).ToListAsync();

            if (userAppraisal == null)
                return employeeAppraisal;

            employeeAppraisal.Id = userAppraisal.Id;
            employeeAppraisal.AppraisalPeriod = userAppraisal.AppraisalPeriod.AppraisalPeriodName;
            employeeAppraisal.ManagerComments = userAppraisal.Comments;
            employeeAppraisal.DevelopmentAreas = userAppraisal.AreaForDevelopment;
            employeeAppraisal.Goals = userAppraisal.GoalsForNextPeriod;
            employeeAppraisal.EmployeeId = userAppraisal.User.EmployeeNumber.HasValue ? userAppraisal.User.EmployeeNumber.Value.ToString() : string.Empty;
            employeeAppraisal.EmployeeName = userAppraisal.User.FullName;
            employeeAppraisal.Department = assignedDepartments.Count > 0 ? string.Join(',', assignedDepartments) : string.Empty;
            employeeAppraisal.Status = userAppraisal.Status;
            employeeAppraisal.Position = assignedRoles.Count > 0 ? string.Join(',', assignedRoles) : string.Empty;

            foreach (var detail in userAppraisal.UserAppraisalDetails)
            {
                employeeAppraisal.AppraisalCriteria.Add(new Application.DTOs.Appraisal.AppraisalCriteriaDTO
                {
                    Id = detail.Id,
                    CriteriaName = detail.UserAppraisalCriteria.CriteriaName,
                    AppraisalID = detail.AppraisalID,
                    CriteriaId = detail.CriteriaID,
                    Rating = (double)detail.Rating,
                    Comments = detail.Comment
                });
            }

            return employeeAppraisal;

        }
    }
}
