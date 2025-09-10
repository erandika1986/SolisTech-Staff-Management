using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StaffApp.Application.Contracts;
using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;
using StaffApp.Application.Extensions.Constants;
using StaffApp.Application.Extensions.Helpers;
using StaffApp.Application.Services;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Enum;
using System.Text;

namespace StaffApp.Infrastructure.Services
{
    public class UserService(
        IStaffAppDbContext context,
        IDepartmentService departmentService,
        IRoleService roleService,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        SignInManager<ApplicationUser> signInManager) : IUserService
    {


        public async Task<GeneralResponseDTO> CreateNewUserAsync(UserDTO model)
        {
            try
            {
                if (await FindUserByEmailAsync(model.Email) is not null)
                    return new GeneralResponseDTO(false, "User already exists");

                var password = string.Join("", new PasswordGenerator().GeneratePasswords(10));
                var maxEmployeeNumber = await context.ApplicationUsers.MaxAsync(x => x.EmployeeNumber);
                var user = new ApplicationUser()
                {
                    FullName = model.FullName,
                    EmployeeNumber = maxEmployeeNumber + 1,
                    Email = model.Email,
                    UserName = model.Email,
                    PasswordHash = password,
                    PhoneNumber = model.PhoneNumber,
                    BirthDate = model.BirthDate,
                    Gender = (Gender)model.SelectedGender.Id,
                    HireDate = model.HireDate,
                    ConfirmationDate = model.ConfirmationDate,
                    NICNumber = model.NICNumber,
                    LandNumber = model.LandNumber,
                    MaritalStatus = (MaritalStatus)model.SelectedMaritalStatus.Id,
                    EmployeeTypeId = model.SelectedEmploymentType.Id,
                    CompanyLocationId = model.SelectedCompanyLocation.Id,
                    EmergencyContactPerson = model.EmergencyContactPerson,
                    EmergencyContactPersonRelationship = model.EmergencyContactPersonRelationship,
                    EmergencyContactPersonPhoneNumber = model.EmergencyContactPersonPhoneNumber,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, password);

                var error = CheckResponse(result);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponseDTO(false, error);

                if (model.SelectedRole.Id != ApplicationConstants.EmptyGuide)
                {
                    await AssignUserToRoleAsync(user, new ApplicationRole(model.SelectedRole.Name) { IsManagerTypeRole = model.SelectedRole.IsManagerTypeRole });
                }

                await departmentService.AddUsersToDepartments(model.Departments.Select(x => x.Id).ToList(), user.Id);



                return new GeneralResponseDTO(true, "A new user has been created successfully.", user.Id);
            }
            catch (Exception ex)
            {

                return new GeneralResponseDTO(false, ex.Message);
            }
            finally
            {
            }
        }

        public async Task<GeneralResponseDTO> UpdateExistingUserAsync(UserDTO model)
        {
            try
            {
                var user = await userManager.FindByIdAsync(model.Id);

                if (user == null)
                    return new GeneralResponseDTO(false, "User not found");

                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.BirthDate = model.BirthDate;
                user.HireDate = model.HireDate;
                user.ConfirmationDate = model.ConfirmationDate;
                user.NICNumber = model.NICNumber;
                user.LandNumber = model.LandNumber;
                user.MaritalStatus = (MaritalStatus)model.SelectedMaritalStatus.Id;
                user.EmployeeTypeId = model.SelectedEmploymentType.Id;
                user.CompanyLocationId = model.SelectedCompanyLocation.Id;
                user.Gender = (Gender)model.SelectedGender.Id;
                user.IsActive = true;
                user.EmergencyContactPerson = model.EmergencyContactPerson;
                user.EmergencyContactPersonRelationship = model.EmergencyContactPersonRelationship;
                user.EmergencyContactPersonPhoneNumber = model.EmergencyContactPersonPhoneNumber;

                await userManager.UpdateAsync(user);

                await ChangeUserRoleAsync(user, new List<string> { model.SelectedRole.Name });


                var savedDepartments = (await departmentService
                    .GetDepartmentsByUserId(user.Id, true)).Select(x => x.Id)
                    .ToList();

                var assignedDepartments = model.Departments.Select(x => x.Id).ToList();
                var newDepartments = assignedDepartments.Except(savedDepartments).ToList();
                var removedDepartments = savedDepartments.Except(assignedDepartments).ToList();


                await departmentService.AddUsersToDepartments(newDepartments, user.Id);
                await departmentService.RemoveUsersFromDepartments(removedDepartments, user.Id);


                return new GeneralResponseDTO(true, "User updated successfully");
            }
            catch (Exception ex)
            {

                return new GeneralResponseDTO(false, ex.Message);
            }

        }

        public async Task<PaginatedResultDTO<BasicUserDTO>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string searchString = null,
            string sortField = null,
            bool ascending = true,
            bool status = true)
        {
            IQueryable<ApplicationUser> query = context.ApplicationUsers.Where(x => x.IsActive == status);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.UserName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.PhoneNumber.Contains(searchString));
            }

            int totalCount = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortField))
            {
                switch (sortField.ToLower())
                {
                    case "username":
                        query = ascending
                            ? query.OrderBy(u => u.UserName)
                            : query.OrderByDescending(u => u.UserName);
                        break;
                    case "email":
                        query = ascending
                            ? query.OrderBy(u => u.Email)
                            : query.OrderByDescending(u => u.Email);
                        break;
                    // Add other sort fields as needed
                    default:
                        query = ascending
                            ? query.OrderBy(u => u.Id)
                            : query.OrderByDescending(u => u.Id);
                        break;
                }
            }
            else
            {
                // Default sorting by username
                query = query.OrderBy(u => u.UserName);
            }

            var items = await query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .Select(user => new BasicUserDTO
                        {
                            UserName = user.UserName ?? string.Empty,
                            FullName = user.FullName ?? string.Empty,
                            EmployeeNo = user.EmployeeNumber.ToString(),
                            Gender = !user.Gender.HasValue ? string.Empty : user.Gender.Value.ToString(),
                            HireDate = !user.HireDate.HasValue ? string.Empty : user.HireDate.Value.ToString("MM/dd/yyyy"),
                            Id = user.Id,
                            NIC = user.NICNumber ?? string.Empty,
                            Phone = user.PhoneNumber ?? string.Empty,
                        })
                        .ToListAsync();

            var newResult = new PaginatedResultDTO<BasicUserDTO>
            {
                Items = items,
                TotalItems = totalCount,
                Page = pageNumber,
                PageSize = pageSize
            };

            return newResult;
        }

        public async Task<List<BasicUserDTO>> GetAllActiveUsersAsync()
        {
            IQueryable<ApplicationUser> query = context.ApplicationUsers.Where(x => x.IsActive == true);

            var items = await query
                        .Select(user => new BasicUserDTO
                        {
                            UserName = user.UserName ?? string.Empty,
                            FullName = user.FullName ?? string.Empty,
                            EmployeeNo = user.EmployeeNumber.ToString(),
                            Gender = !user.Gender.HasValue ? string.Empty : user.Gender.Value.ToString(),
                            HireDate = !user.HireDate.HasValue ? string.Empty : user.HireDate.Value.ToString("MM/dd/yyyy"),
                            Id = user.Id,
                            NIC = user.NICNumber ?? string.Empty,
                            Phone = user.PhoneNumber ?? string.Empty,
                        })
                        .ToListAsync();

            return items;
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            var userDepartments = await departmentService
                .GetDepartmentsByUserId(id, true);

            var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            var role = await roleService.GetRoleByNameAsync(userRole);

            return new UserDTO()
            {
                FullName = user.FullName,
                EmergencyContactPerson = user.EmergencyContactPerson,
                EmergencyContactPersonRelationship = user.EmergencyContactPersonRelationship,
                EmergencyContactPersonPhoneNumber = user.EmergencyContactPersonPhoneNumber,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate ?? DateTime.Now,
                SelectedGender = new DropDownDTO() { Id = user.Gender.HasValue ? (int)user.Gender.Value : (int)Gender.Unknown, Name = string.Empty },
                HireDate = user.HireDate ?? DateTime.Now,
                ConfirmationDate = user.ConfirmationDate ?? null,
                NICNumber = user.NICNumber,
                LandNumber = user.LandNumber,
                SelectedMaritalStatus = user.MaritalStatus > 0 ? new DropDownDTO() { Id = (int)user.MaritalStatus, Name = EnumHelper.GetEnumDescription(user.MaritalStatus) } : new DropDownDTO(),
                IsActive = user.IsActive,
                SelectedRole = role is not null ? new RoleDTO() { Id = role.Id, Name = role.Name, IsManagerTypeRole = role.IsManagerTypeRole.Value } : new RoleDTO() { Id = ApplicationConstants.EmptyGuide },
                DepartmentIds = userDepartments.Select(x => x.Id).ToList(),
                SelectedEmploymentType = new DropDownDTO()
                {
                    Id = user.EmployeeTypeId.HasValue ? (int)user.EmployeeTypeId.Value : 0,
                },
                SelectedCompanyLocation = user.CompanyLocationId.HasValue ? new DropDownDTO() { Id = user.CompanyLocationId.Value } : new DropDownDTO(),
                Id = user.Id
            };
        }

        public async Task<List<RoleDTO>> GetAvailableRoles()
        {
            var allRoles = await roleService.GetAllRolesAsync();

            return allRoles.OrderBy(x => x.Name).Adapt<List<RoleDTO>>();
        }

        public async Task<List<RoleDropDownDTO>> GetAvailableRolesWithDefaultHourlyRate()
        {
            var allRoles = await roleService.GetAllRolesAsync();

            return allRoles.OrderBy(x => x.Name).Select(x => new RoleDropDownDTO()
            {
                Id = x.Id,
                Name = x.Name,
                HourlyRate = x.DefaultHourlyRate
            }).ToList();
        }

        public async Task<GeneralResponseDTO> DeleteUser(string id)
        {
            var user = await context.ApplicationUsers.FindAsync(id);
            user.IsActive = false;
            context.ApplicationUsers.Update(user);
            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO(true, "User has been deleted successfully.");
        }



        public async Task<List<UserDropDownDTO>> GetManagerJobRoleUsersAsync()
        {
            var roleNames = new List<string> { RoleConstants.Director, RoleConstants.Manager, RoleConstants.TeamLead };
            var users = new List<ApplicationUser>();
            foreach (var roleName in roleNames)
            {
                if (await roleService.RoleExistsAsync(roleName))
                {
                    var usersInRole = await userManager.GetUsersInRoleAsync(roleName);
                    users.AddRange(usersInRole);
                }
            }

            var userDto = users.Distinct().Select(x => new UserDropDownDTO()
            {
                Id = x.Id,
                Name = x.FullName
            }).ToList();

            return userDto;
        }

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

        public async Task<List<string>> GetLoggedInUserAssignedRoles(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return new List<string>();

            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<GeneralResponseDTO> SaveUserBankAccount(UserBankAccountDTO userBankAccount)
        {
            try
            {
                var bankAccount = await context.EmployeeBankAccounts.FindAsync(userBankAccount.Id);

                if (userBankAccount.IsPrimaryAccount)
                {
                    var primaryBankAccount = await context.EmployeeBankAccounts
                        .Where(x =>
                            x.EmployeeId == bankAccount.EmployeeId &&
                            x.Id != userBankAccount.Id &&
                            x.IsActive &&
                            x.IsPrimaryAccount == true)
                        .ToListAsync();

                    foreach (var item in primaryBankAccount)
                    {
                        item.IsPrimaryAccount = false;
                        context.EmployeeBankAccounts.Update(item);
                    }
                }

                if (bankAccount is null)
                {
                    bankAccount = new Domain.Entity.EmployeeBankAccount();
                    bankAccount.EmployeeId = userBankAccount.EmployeeId;
                    bankAccount.AccountName = userBankAccount.AccountName;
                    bankAccount.AccountNumber = userBankAccount.AccountNumber;
                    bankAccount.BankName = userBankAccount.BankName;
                    bankAccount.BranchCode = userBankAccount.BranchCode;
                    bankAccount.BranchName = userBankAccount.BranchName;
                    bankAccount.IsPrimaryAccount = userBankAccount.IsPrimaryAccount;
                    bankAccount.CreatedByUserId = currentUserService.UserId;
                    bankAccount.UpdateDate = DateTime.Now;
                    bankAccount.CreatedDate = DateTime.Now;
                    bankAccount.UpdatedByUserId = currentUserService.UserId;
                    bankAccount.IsActive = true;
                    context.EmployeeBankAccounts.Add(bankAccount);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO(true, "User bank account has been created successfully.");
                }
                else
                {
                    bankAccount.AccountName = userBankAccount.AccountName;
                    bankAccount.AccountNumber = userBankAccount.AccountNumber;
                    bankAccount.BankName = userBankAccount.BankName;
                    bankAccount.BranchCode = userBankAccount.BranchCode;
                    bankAccount.BranchName = userBankAccount.BranchName;
                    bankAccount.UpdateDate = DateTime.Now;
                    bankAccount.UpdatedByUserId = currentUserService.UserId;
                    bankAccount.IsPrimaryAccount = userBankAccount.IsPrimaryAccount;
                    context.EmployeeBankAccounts.Update(bankAccount);

                    await context.SaveChangesAsync(CancellationToken.None);

                    return new GeneralResponseDTO(true, "User bank account has been updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponseDTO(false, ex.Message);
            }
        }

        public Task<List<UserBankAccountDTO>> GetAllUserBankAccount(string userId)
        {
            var bankAccounts = context.EmployeeBankAccounts
                 .Where(x => x.EmployeeId == userId && x.IsActive)
                 .Select(x => new UserBankAccountDTO()
                 {
                     Id = x.Id,
                     EmployeeId = x.EmployeeId,
                     BankName = x.BankName,
                     AccountName = x.AccountName,
                     AccountNumber = x.AccountNumber,
                     BranchName = x.BranchName,
                     BranchCode = x.BranchCode,
                     IsPrimaryAccount = x.IsPrimaryAccount
                 }).ToListAsync();
            return bankAccounts;
        }

        public async Task<GeneralResponseDTO> DeleteUserBankAccount(int id)
        {
            var bankAccount = await context.EmployeeBankAccounts.FindAsync(id);

            if (bankAccount is null)
                return new GeneralResponseDTO(false, "Bank account not found");

            bankAccount.IsActive = false;
            bankAccount.UpdateDate = DateTime.Now;
            bankAccount.UpdatedByUserId = currentUserService.UserId;

            if (bankAccount.IsPrimaryAccount)
            {
                var primaryBankAccount = await context.EmployeeBankAccounts
                    .Where(x => x.EmployeeId == bankAccount.EmployeeId && x.IsActive && x.IsPrimaryAccount != true && x.Id != bankAccount.Id)
                    .FirstOrDefaultAsync();
                if (primaryBankAccount != null)
                {
                    primaryBankAccount.IsPrimaryAccount = true;
                    context.EmployeeBankAccounts.Update(primaryBankAccount);
                }
                bankAccount.IsPrimaryAccount = false;
            }

            context.EmployeeBankAccounts.Update(bankAccount);
            await context.SaveChangesAsync(CancellationToken.None);

            return new GeneralResponseDTO(true, "User bank account has been deleted successfully.");
        }

        private async Task<ApplicationUser?> FindUserByEmailAsync(string email) => await userManager.FindByEmailAsync(email);

        private static string CheckResponse(IdentityResult result)
        {
            if (result.Succeeded)
                return string.Empty;
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    sb.Append(error.Description);
                }
                return sb.ToString();
            }
        }

        private async Task<GeneralResponseDTO> ChangeUserRoleAsync(ApplicationUser user, List<string> currentRoles)
        {
            var previousRoles = (await userManager.GetRolesAsync(user)).ToList();

            var newlyAddedRoles = currentRoles.Except(previousRoles).ToList();

            var removedRoles = previousRoles.Except(currentRoles).ToList();

            foreach (var role in removedRoles)
            {
                var removeOldRole = await userManager.RemoveFromRoleAsync(user, role);
                var error = CheckResponse(removeOldRole);
                if (!string.IsNullOrEmpty(error))
                    return new GeneralResponseDTO(false, error);
            }

            foreach (var role in newlyAddedRoles)
            {
                var applicationRole = await roleService.GetRoleByNameAsync(role);
                var result = await AssignUserToRoleAsync(user, applicationRole);
                if (!result.Flag)
                    return result;
            }

            return new GeneralResponseDTO(true, "User role updated successfully");
        }

        private async Task<GeneralResponseDTO> AssignUserToRoleAsync(ApplicationUser user, ApplicationRole role)
        {
            if (user is null || role is null)
                return new GeneralResponseDTO(false, "Model state can't be empty");

            if (await roleService.GetRoleByNameAsync(role.Name) is null)
                await roleService.CreateRoleAsync(role.Name, role.IsManagerTypeRole.Value, role.DefaultHourlyRate);

            IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
            string error = CheckResponse(result);
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponseDTO(false, error);
            else
                return new GeneralResponseDTO(true, $"{user.FullName} assigned to {role.Name} successfully");
        }
    }
}
