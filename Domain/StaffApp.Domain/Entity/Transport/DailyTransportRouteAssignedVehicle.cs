using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Domain.Entity.Transport
{
    public class DailyTransportRouteAssignedVehicle : BaseAuditableEntity
    {
        public int DailyTransportRouteId { get; set; }
        public int VehicleId { get; set; }

        public virtual DailyTransportRoute DailyTransportRoute { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
