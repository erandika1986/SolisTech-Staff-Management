using Microsoft.AspNetCore.Identity;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Application.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<RoleDTO> GetRoleByIdAsync(string roleId);
        Task<ApplicationRole> GetRoleByNameAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName, bool isManagerTypeRole, decimal? hourlyRate);
        Task<IdentityResult> UpdateRoleAsync(RoleDTO role);
        Task<IdentityResult> DeleteRoleAsync(string roleId);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IEnumerable<string>> GetUsersInRoleAsync(string roleName);
        Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
        Task<IEnumerable<UserDropDownDTO>> GetUserDropDownsInRoleAsync(string roleName);
    }
}
