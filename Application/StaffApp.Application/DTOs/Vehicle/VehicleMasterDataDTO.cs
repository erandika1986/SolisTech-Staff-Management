using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.Vehicle
{
    public class VehicleMasterDataDTO
    {
        public List<DropDownDTO> VehicleTypes { get; set; } = new();
        public List<DropDownDTO> VehicleOwnerTypes { get; set; } = new();
        public List<DropDownDTO> VehiclePurposes { get; set; } = new();
    }
}
