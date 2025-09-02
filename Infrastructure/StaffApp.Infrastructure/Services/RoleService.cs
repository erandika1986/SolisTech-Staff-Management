using Microsoft.AspNetCore.Identity;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Services
{
    public class RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager) : IRoleService
    {


        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            var allRoles = roleManager.Roles.OrderBy(x => x.Name).Select(x => new RoleDTO
            {
                IsManagerTypeRole = x.IsManagerTypeRole.HasValue ? x.IsManagerTypeRole.Value : false,
                Id = x.Id,
                Name = x.Name,
                DefaultHourlyRate = x.DefaultHourlyRate.HasValue ? (double)x.DefaultHourlyRate.Value : 0.0,
                ManagerTypeRole = (x.IsManagerTypeRole.HasValue && x.IsManagerTypeRole.Value) ? "Yes" : "No",

            }).ToList();

            return allRoles;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            return new RoleDTO()
            {
                Id = role.Id,
                Name = role.Name,
                IsManagerTypeRole = role.IsManagerTypeRole.HasValue ? role.IsManagerTypeRole.Value : false,
                DefaultHourlyRate = role.DefaultHourlyRate.HasValue ? (double)role.DefaultHourlyRate.Value : 0.0
            };
        }


        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            return await roleManager.FindByNameAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName, bool isManagerTypeRole, decimal? hourlyRate)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty", nameof(roleName));

            var roleExists = await RoleExistsAsync(roleName);
            if (roleExists)
                throw new InvalidOperationException($"Role '{roleName}' already exists.");

            var role = new ApplicationRole(roleName)
            {
                IsManagerTypeRole = isManagerTypeRole,
                DefaultHourlyRate = hourlyRate  // Assuming default hourly rate is only set for manager type roles
            };
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleAsync(RoleDTO role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var savedRole = await roleManager.FindByIdAsync(role.Id);
            savedRole.Name = role.Name;
            savedRole.IsManagerTypeRole = role.IsManagerTypeRole;
            savedRole.DefaultHourlyRate = (decimal)role.DefaultHourlyRate;

            return await roleManager.UpdateAsync(savedRole);
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new InvalidOperationException($"Role with ID '{roleId}' not found.");

            return await roleManager.DeleteAsync(role);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }

        public async Task<IEnumerable<string>> GetUsersInRoleAsync(string roleName)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
            var userIds = new List<string>();

            foreach (var user in usersInRole)
            {
                userIds.Add(user.Id);
            }

            return userIds;
        }

        public async Task<IEnumerable<UserDropDownDTO>> GetUserDropDownsInRoleAsync(string roleName)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
            var users = new List<UserDropDownDTO>();

            foreach (var user in usersInRole)
            {
                users.Add(new UserDropDownDTO() { Id = user.Id, Name = user.FullName });
            }

            return users;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found.");

            if (!await RoleExistsAsync(roleName))
                throw new InvalidOperationException($"Role '{roleName}' does not exist.");

            if (await userManager.IsInRoleAsync(user, roleName))
                return IdentityResult.Success; // User is already in role

            return await userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found.");

            return await userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID '{userId}' not found.");

            return await userManager.IsInRoleAsync(user, roleName);
        }
    }
}
