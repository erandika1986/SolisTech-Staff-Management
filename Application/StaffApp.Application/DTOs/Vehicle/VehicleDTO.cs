using StaffApp.Application.DTOs.Common;

namespace StaffApp.Application.DTOs.Vehicle
{
    public class VehicleDTO
    {
        public int Id { get; set; }
        public string Registration { get; set; }
        public int Capacity { get; set; }
        public DropDownDTO VehicleType { get; set; }
        public DropDownDTO VehicleOwner { get; set; }
        public string ManufactureName { get; set; }
        public decimal? MonthlyRent { get; set; }
        public string? CreatedOn { get; set; }

        //private HashSet<Purpose> _selectedPurposes = new();
        public IEnumerable<DropDownDTO> AssignedPurposes { get; set; } = new HashSet<DropDownDTO>();
    }

}
