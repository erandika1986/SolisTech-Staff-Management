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
    }
}
