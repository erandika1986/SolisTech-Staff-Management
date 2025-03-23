using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Department;

namespace StaffApp.Application.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDTO>> GetAllDepartments(string searchText, bool isActive = true);
        Task<DepartmentDTO> GetDepartmentById(int id);
        Task<GeneralResponseDTO> SaveDepartment(DepartmentDTO model);
        Task<GeneralResponseDTO> DeleteDepartment(int id);
        Task<GeneralResponseDTO> AddUsersToDepartments(List<int> departments, string userId);
        Task<GeneralResponseDTO> RemoveUsersFromDepartments(List<int> departments, string userId);
        Task<List<DepartmentDTO>> GetDepartmentsByUserId(string userId, bool isActive);
    }
}
