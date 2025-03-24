using Microsoft.AspNetCore.Identity;
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
                ManagerTypeRole = (x.IsManagerTypeRole.HasValue && x.IsManagerTypeRole.Value) ? "Yes" : "No",

            }).ToList();

            return allRoles;
        }

        public async Task<RoleDTO> GetRoleByIdAsync(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            return new RoleDTO() { Id = role.Id, Name = role.Name, IsManagerTypeRole = role.IsManagerTypeRole.HasValue ? role.IsManagerTypeRole.Value : false };
        }


        public async Task<ApplicationRole> GetRoleByNameAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName, bool isManagerTypeRole)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Role name cannot be null or empty", nameof(roleName));

            var roleExists = await RoleExistsAsync(roleName);
            if (roleExists)
                throw new InvalidOperationException($"Role '{roleName}' already exists.");

            var role = new ApplicationRole(roleName) { IsManagerTypeRole = isManagerTypeRole };
            return await roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleAsync(ApplicationRole role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return await roleManager.UpdateAsync(role);
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
