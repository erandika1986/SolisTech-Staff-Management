using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Transport;

namespace StaffApp.Application.Services
{
    public interface IRouteService
    {
        Task<GeneralResponseDTO> SaveRouteAsync(RouteDTO model);
        Task<GeneralResponseDTO> DeleteRouteAsync(int id);
        Task<List<RouteDTO>> GetAllRoutesAsync(string name);
    }
}
