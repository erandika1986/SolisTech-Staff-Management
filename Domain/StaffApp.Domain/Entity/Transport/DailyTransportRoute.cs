using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Transport
{
    public class DailyTransportRoute : BaseAuditableEntity
    {
        public int PredefineRouteId { get; set; }
        public DateTime Date { get; set; }

        public virtual PredefineRoute PredefineRoute { get; set; }

        public virtual ICollection<DailyTransportRouteSeat> DailyTransportRouteSeats { get; set; } = new HashSet<DailyTransportRouteSeat>();
        public virtual ICollection<DailyTransportRouteAssignedVehicle> DailyTransportRouteAssignedVehicles { get; set; } = new HashSet<DailyTransportRouteAssignedVehicle>();
    }
}
