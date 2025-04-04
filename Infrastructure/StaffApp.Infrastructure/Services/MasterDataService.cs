using Mapster;
using Microsoft.EntityFrameworkCore;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Services
{
    public class MasterDataService(
        IStaffAppDbContext context,
        IRoleService roleService) : IMasterDataService
    {
        public async Task<List<DropDownDTO>> GetAvailableEmploymentTypes()
        {
            var employmentTypes = await context
                .EmployeeTypes
                .Select(x => new DropDownDTO() { Id = x.Id, Name = x.Name })
                .ToListAsync();

            return employmentTypes;
        }

        public List<DropDownDTO> GetAvailableGenderTypes()
        {
            return EnumHelper.GetDropDownList<Gender>();
        }

        public List<DropDownDTO> GetAllowGenderTypes()
        {
            return new List<DropDownDTO>()
            {
                new DropDownDTO() { Id = ApplicationConstants.Zero, Name = "All" },
                new DropDownDTO() { Id = (int)Gender.Male, Name = EnumHelper.GetEnumDescription(Gender.Male) },
                new DropDownDTO() { Id = (int)Gender.Female, Name = EnumHelper.GetEnumDescription(Gender.Female) },
            };
        }

        public async Task<List<RoleDTO>> GetAvailableRoles()
        {
            var allRoles = await roleService.GetAllRolesAsync();

            return allRoles.OrderBy(x => x.Name).Adapt<List<RoleDTO>>();
        }

        public async Task<List<DropDownDTO>> GetLeaveTypes(bool hasDefaultValue = false)
        {
            var response = new List<DropDownDTO>();
            if (hasDefaultValue)
                response.Add(new DropDownDTO() { Id = 0, Name = "All" });

            var leaveTypes = await context
                .LeaveTypes
                .Where(x => x.IsActive)
                .Select(x => new DropDownDTO()
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToListAsync();

            response.AddRange(leaveTypes);

            return response;
        }

        public async Task<List<DropDownDTO>> GetAvailableCompanyYears()
        {
            var response = new List<DropDownDTO>();
            try
            {
                var companyCurrentYear = await context.CompanyYears.FirstOrDefaultAsync(x => x.IsCurrentYear);
                var currentYearId = 0;
                if (companyCurrentYear is not null)
                {
                    currentYearId = companyCurrentYear.Id;
                    response.Add(new DropDownDTO() { Id = companyCurrentYear.Id, Name = companyCurrentYear.Year.ToString() });
                }

                var otherRegisteredYears = await context
                    .CompanyYears
                    .Where(x => x.Id != currentYearId).OrderBy(x => x.Id)
                    .Select(x => new DropDownDTO()
                    {
                        Id = x.Id,
                        Name = x.Year.ToString()
                    }).ToListAsync();

                if (otherRegisteredYears.Any())
                    response.AddRange(otherRegisteredYears);
            }
            catch (Exception ex)
            {
            }


            return response;
        }

        public List<DropDownDTO> GetLeaveStatuses(bool hasDefaultValue = false)
        {
            var response = new List<DropDownDTO>();
            if (hasDefaultValue)
                response.Add(new DropDownDTO() { Id = 0, Name = "All" });

            response.AddRange(EnumHelper.GetDropDownList<LeaveStatus>());

            return response;
        }

        public async Task<List<DropDownDTO>> GetLeaveDurations(int leaveTypeId)
        {
            var response = new List<DropDownDTO>();

            var leaveDurations = await context.LeaveTypeAllowDurations
                .Where(x => x.LeaveTypeId == leaveTypeId)
                .ToListAsync();

            foreach (var leaveDuration in leaveDurations)
            {
                response.Add(new DropDownDTO() { Id = (int)leaveDuration.LeaveDuration, Name = EnumHelper.GetEnumDescription(leaveDuration.LeaveDuration) });
            }

            return response;
        }

        public List<DropDownDTO> GetHalfDaySessionTypes()
        {
            return EnumHelper.GetDropDownList<HalfDaySessionType>();
        }

        public List<DropDownDTO> GetShortLeaveSessionTypes()
        {
            return EnumHelper.GetDropDownList<ShortLeaveSessionType>();
        }

        public async Task<List<DropDownDTO>> GetOfficeLocations()
        {
            var locations = await context.CompanyLocations.Where(x => x.IsActive)
                .Select(x => new DropDownDTO()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();

            return locations;
        }
    }
}
