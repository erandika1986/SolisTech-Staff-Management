using StaffApp.Application.DTOs.Common;
using StaffApp.Application.DTOs.Vehicle;

namespace StaffApp.Application.Services
{
    public interface IVehicleService
    {
        Task<GeneralResponseDTO> SaveVehicle(VehicleDTO vehicleDTO);
        Task<List<VehicleDTO>> GetAllVehicles();
        Task<GeneralResponseDTO> DeleteVehicle(int vehicleId);
        Task<VehicleMasterDataDTO> GetVehicleMasterData();
    }
}
