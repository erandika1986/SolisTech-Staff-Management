using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;

namespace StaffApp.Application.Services
{
    public interface IUserService
    {
        Task<PaginatedResultDTO<BasicUserDTO>> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string searchString = null,
            string sortField = null,
            bool ascending = true,
            bool status = true);

        Task<UserDTO> GetUserByIdAsync(string id);
        Task<List<UserDropDownDTO>> GetManagerJobRoleUsersAsync();
        Task<GeneralResponseDTO> CreateNewUserAsync(UserDTO model);
        Task<GeneralResponseDTO> UpdateExistingUserAsync(UserDTO model);
        Task<List<RoleDTO>> GetAvailableRoles();
        Task<GeneralResponseDTO> DeleteUser(string id);
        Task<List<DropDownDTO>> GetAvailableEmploymentTypes();
        List<DropDownDTO> GetAvailableGenderTypes();
        Task<List<string>> GetLoggedInUserAssignedRoles(string id);
        Task<GeneralResponseDTO> SaveUserBankAccount(UserBankAccountDTO userBankAccount);
        Task<List<UserBankAccountDTO>> GetAllUserBankAccount(string userId);
        Task<GeneralResponseDTO> DeleteUserBankAccount(int id);
    }
}
