using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Transport;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity.Master
{
    public class Vehicle : BaseAuditableEntity
    {
        public string Registration { get; set; }
        public int Capacity { get; set; }
        public VehicleType VehicleType { get; set; }
        public VehicleOwner VehicleOwner { get; set; }
        public string ManufactureName { get; set; }
        public decimal? MonthlyRent { get; set; }

        public virtual ICollection<DailyTransportRouteAssignedVehicle> DailyTransportRouteAssignedVehicles { get; set; } = new HashSet<DailyTransportRouteAssignedVehicle>();
        public virtual ICollection<VehicleAssignedPurpose> VehicleAssignedPurposes { get; set; } = new HashSet<VehicleAssignedPurpose>();
    }
}
