using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.User;

namespace StaffApp.Application.Services
{
    public interface IMasterDataService
    {
        Task<List<DropDownDTO>> GetAvailableEmploymentTypes();
        List<DropDownDTO> GetAvailableGenderTypes();
        List<DropDownDTO> GetAllowGenderTypes();
        Task<List<RoleDTO>> GetAvailableRoles();
    }
}
