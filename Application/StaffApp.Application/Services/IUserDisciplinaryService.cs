using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.DisciplinaryAction;

namespace StaffApp.Application.Services
{
    public interface IUserDisciplinaryService
    {
        Task<DisciplinaryActionDTO> GetByIdAsync(int id);
        Task<PaginatedResultDTO<DisciplinaryActionDTO>> GetByUserIdAsync(int pageNumber, int pageSize, string userId);
        Task<PaginatedResultDTO<DisciplinaryActionDTO>> GetAllAsync(int pageNumber, int pageSize, string searchText);
        Task<GeneralResponseDTO> AddAsync(DisciplinaryActionDTO action);
        Task<GeneralResponseDTO> UpdateAsync(DisciplinaryActionDTO action);
        Task<GeneralResponseDTO> DeleteAsync(int id);
        DisciplinaryActionMasterDTO GetDisciplinaryActionMasterData();
    }
}
