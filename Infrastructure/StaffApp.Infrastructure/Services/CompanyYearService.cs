using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.CompanyYear;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Services
{
    public class CompanyYearService(IStaffAppDbContext context, ICurrentUserService currentUserService) : ICompanyYearService
    {
        public async Task<GeneralResponseDTO> DeleteCompanyYear(int id)
        {
            var companyYear = await context.CompanyYears.FindAsync(id);

            if (companyYear == null)
                return new GeneralResponseDTO { Flag = false, Message = "Company year not found" };

            try
            {
                companyYear.IsActive = false;
                companyYear.UpdateDate = DateTime.Now;
                companyYear.UpdatedByUserId = currentUserService.UserId;
                context.CompanyYears.Update(companyYear);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO { Flag = true, Message = "Company year deleted successfully" };
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO { Flag = false, Message = ex.Message };
            }
        }

        public async Task<List<CompanyYearDTO>> GetAllCompanyYears(bool isActive = true)
        {
            IQueryable<CompanyYear> query = context
                .CompanyYears
                .OrderByDescending(x => x.Year);

            var companyYears = await query.Where(x => x.IsActive == isActive).Select(d => new CompanyYearDTO
            {
                Id = d.Id,
                Year = d.Year,
                EndDate = d.EndDate,
                StartDate = d.StartDate,
                CurrentYear = d.IsCurrentYear ? "Yes" : "No",
                IsCurrentYear = d.IsCurrentYear
            }).ToListAsync();

            return companyYears;
        }

        public async Task<CompanyYearDTO> GetCompanyYearById(int id)
        {
            var companyYear = await context.CompanyYears.FindAsync(id);

            return companyYear != null ? new CompanyYearDTO
            {
                Id = companyYear.Id,
                Year = companyYear.Year,
                EndDate = companyYear.EndDate,
                StartDate = companyYear.StartDate,
                CurrentYear = companyYear.IsCurrentYear ? "Yes" : "No",
                IsCurrentYear = companyYear.IsCurrentYear
            } : null;
        }

        public async Task<GeneralResponseDTO> SaveCompanyYear(CompanyYearDTO model)
        {
            var companyYear = await context.CompanyYears.FindAsync(model.Year);
            if (companyYear != null)
            {
                companyYear.Year = model.Year;
                companyYear.StartDate = model.StartDate.Value;
                companyYear.EndDate = model.EndDate.Value;
                companyYear.UpdateDate = DateTime.Now;
                companyYear.UpdatedByUserId = currentUserService.UserId;

                context.CompanyYears.Update(companyYear);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO { Flag = true, Message = "Company year updated successfully" };
            }
            else
            {
                companyYear = new Domain.Entity.CompanyYear
                {
                    Id = model.Year,
                    Year = model.Year,
                    StartDate = model.StartDate.Value,
                    EndDate = model.EndDate.Value,
                    IsCurrentYear = false,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = currentUserService.UserId,
                    UpdateDate = DateTime.Now,
                    UpdatedByUserId = currentUserService.UserId,
                };

                context.CompanyYears.Add(companyYear);
                await context.SaveChangesAsync(CancellationToken.None);

                return new GeneralResponseDTO { Flag = true, Message = "A company year added successfully" };
            }
        }
    }
}
