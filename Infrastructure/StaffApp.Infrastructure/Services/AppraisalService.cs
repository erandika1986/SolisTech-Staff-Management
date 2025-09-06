using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Appraisal;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Services
{
    public class AppraisalService(IStaffAppDbContext context,
        ICurrentUserService currentUserService,
        ILogger<IAppraisalService> logger) : IAppraisalService
    {
        public async Task<GeneralResponseDTO> GenerateAppraisalDataForSelectedCompanyYear(int companyYearId)
        {
            try
            {
                var appraisalPeriod = await context.AppraisalPeriods
                    .FirstOrDefaultAsync(x => x.CompanyYearId == companyYearId);

                var activeEmployees = await context.ApplicationUsers
                    .Where(x => x.IsActive)
                    .ToListAsync(CancellationToken.None);

                var userAppraisalCriterias = await context.UserAppraisalCriterias
                    .Where(x => x.IsActive == true)
                    .ToListAsync(CancellationToken.None);

                if (appraisalPeriod is null)
                {
                    appraisalPeriod = new Domain.Entity.AppraisalPeriod()
                    {
                        AppraisalPeriodName = $"Annual Appraisal({companyYearId})",
                        AppraisalStatus = Domain.Enum.AppraisalStatus.Pending,
                        CompanyYearId = companyYearId,
                        EndDate = new DateTime(companyYearId, 12, 31),
                        StartDate = new DateTime(companyYearId, 1, 1),
                    };

                    foreach (var activeEmployee in activeEmployees)
                    {
                        var userAppraisal = await GenerateNewUserAppraisalRecord(activeEmployee, userAppraisalCriterias);

                        appraisalPeriod.UserAppraisals.Add(userAppraisal);
                    }

                    context.AppraisalPeriods.Add(appraisalPeriod);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO { Flag = true, Message = "User Appraisal Data generated successfully." };
                }
                else
                {
                    foreach (var activeEmployee in activeEmployees)
                    {
                        var userAppraisal = appraisalPeriod.UserAppraisals.FirstOrDefault(x => x.UserId == activeEmployee.Id);
                        if (userAppraisal is null)
                        {
                            userAppraisal = await GenerateNewUserAppraisalRecord(activeEmployee, userAppraisalCriterias);

                            appraisalPeriod.UserAppraisals.Add(userAppraisal);
                        }
                        else
                        {
                            var newUserAppraisalCriterias = (
                                from c in userAppraisalCriterias
                                where !userAppraisal.UserAppraisalDetails.Any(x => x.CriteriaID == c.Id)
                                select c).ToList();

                            foreach (var newCriterias in newUserAppraisalCriterias)
                            {
                                userAppraisal.UserAppraisalDetails.Add(new UserAppraisalDetail()
                                {
                                    Comment = string.Empty,
                                    CriteriaID = newCriterias.Id,
                                    Rating = 0
                                });
                            }

                            var deletedUserAppraisalCriterias = (
                                from d in userAppraisal.UserAppraisalDetails
                                where userAppraisalCriterias.Any(x => x.Id == d.CriteriaID)
                                select d).ToList();

                            foreach (var deleteCriteria in deletedUserAppraisalCriterias)
                            {
                                userAppraisal.UserAppraisalDetails.Remove(deleteCriteria);
                            }
                        }
                    }

                    context.AppraisalPeriods.Update(appraisalPeriod);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO { Flag = true, Message = "User Appraisal Data re-generated successfully." };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding appraisal period for company year  {companyYearId}", companyYearId);
                return new GeneralResponseDTO
                {
                    Flag = false,
                    Message = "An error occurred while adding the appraisal period."
                };
            }

        }

        public async Task<PaginatedResultDTO<AppraisalPeriodDTO>> GetAppraisalPeriodForSelectedYear(int companyYearId)
        {
            var query = context.CompanyYears.AsQueryable();
            if (companyYearId > 0)
            {
                query = query.Where(x => x.Id == companyYearId);
            }

            var companyYears = await query.ToListAsync();
            var appraisalPeriodList = new List<AppraisalPeriodDTO>();

            foreach (var companyYear in companyYears)
            {
                if (companyYear.AppraisalPeriods.Count == 0)
                {
                    var appraisalPeriod = new AppraisalPeriodDTO()
                    {
                        Id = 0,
                        AppraisalPeriodName = $"Annual Appraisal({companyYear.Id})",
                        AppraisalStatus = "Not Generated",
                        CompanyYearId = companyYear.Id,
                        EndDate = (new DateTime(companyYear.Id, 12, 31)).ToString("MM/dd/yyyy"),
                        StartDate = (new DateTime(companyYear.Id, 1, 1)).ToString("MM/dd/yyyy")
                    };

                    appraisalPeriodList.Add(appraisalPeriod);
                }
                else
                {
                    foreach (var appraisalPeriod in companyYear.AppraisalPeriods)
                    {
                        appraisalPeriodList.Add(new AppraisalPeriodDTO()
                        {
                            Id = appraisalPeriod.Id,
                            AppraisalPeriodName = appraisalPeriod.AppraisalPeriodName,
                            AppraisalStatus = EnumHelper.GetEnumDescription(appraisalPeriod.AppraisalStatus),
                            CompanyYearId = appraisalPeriod.CompanyYearId,
                            EndDate = appraisalPeriod.EndDate.ToString("MM/dd/yyyy"),
                            StartDate = appraisalPeriod.StartDate.ToString("MM/dd/yyyy")
                        });
                    }
                }
            }

            var newResult = new PaginatedResultDTO<AppraisalPeriodDTO>
            {
                Items = appraisalPeriodList,
                TotalItems = 1,
                Page = 1,
                PageSize = 10,
                IsReadOnly = true
            };

            return newResult;
        }

        public async Task<List<UserAppraisalSummaryDTO>> GetMyAssignedAppraisal(int companyYearId)
        {
            var assignedAppraisals = await (from ua in context.UserAppraisals
                                            join ap in context.AppraisalPeriods on ua.AppraisalPeriodId equals ap.Id
                                            join u in context.ApplicationUsers on ua.UserId equals u.Id
                                            join r in context.ApplicationUsers on ua.ReviewerId equals r.Id into reviewers
                                            from reviewer in reviewers.DefaultIfEmpty()
                                            where ap.CompanyYearId == companyYearId && ua.ReviewerId == currentUserService.UserId
                                            select new UserAppraisalSummaryDTO
                                            {
                                                AppraisalPeriod = ap.AppraisalPeriodName,
                                                Id = ua.Id,
                                                Comments = ua.Comments,
                                                OverallRating = ua.OverallRating,
                                                ReviewerName = reviewer != null ? reviewer.FullName : "N/A",
                                                Status = EnumHelper.GetEnumDescription(ua.Status),
                                                UserFullName = u.FullName
                                            }).ToListAsync(CancellationToken.None);

            return assignedAppraisals;
        }

        private async Task<UserAppraisal> GenerateNewUserAppraisalRecord(
            ApplicationUser activeEmployee,
            List<UserAppraisalCriteria> userAppraisalCriterias)
        {
            var assignedDepartments = await context.EmployeeDepartments.Where(x => x.UserId == activeEmployee.Id).ToListAsync(CancellationToken.None);

            var assignedReviewers = assignedDepartments.Select(x => x.Department)
                .Where(x => x.DepartmentHeadId != null)
                .ToList();

            var userAppraisal = new UserAppraisal()
            {
                Comments = string.Empty,
                CreatedByUserId = currentUserService.UserId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                OverallRating = 0,
                Status = Domain.Enum.AppraisalStatus.Pending,
                UpdateDate = DateTime.UtcNow,
                UpdatedByUserId = currentUserService.UserId,
                UserId = activeEmployee.Id,
                ReviewerId = assignedReviewers.Count > 0 ? assignedReviewers.FirstOrDefault().DepartmentHeadId : null
            };

            foreach (var criteria in userAppraisalCriterias)
            {
                userAppraisal.UserAppraisalDetails.Add(new UserAppraisalDetail()
                {
                    Comment = string.Empty,
                    CriteriaID = criteria.Id,
                    Rating = 0
                });
            }

            return userAppraisal;
        }
    }
}
