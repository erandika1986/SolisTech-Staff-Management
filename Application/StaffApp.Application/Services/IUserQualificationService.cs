using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.UserQualification;

namespace StaffApp.Application.Services
{
    public interface IUserQualificationService
    {
        Task<List<UserQualificationDTO>> GetAllAsync(string userId);
        Task<UserQualificationDTO> GetByIdAsync(int id);
        Task<GeneralResponseDTO> CreateAsync(UserQualificationDTO dto);
        Task<GeneralResponseDTO> UpdateAsync(UserQualificationDTO dto);
        Task<GeneralResponseDTO> DeleteAsync(int id);
    }
}
