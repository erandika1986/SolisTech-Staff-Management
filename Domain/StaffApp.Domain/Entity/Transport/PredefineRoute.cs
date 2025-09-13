using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity.Transport
{
    public class PredefineRoute : BaseEntity
    {
        public int RouteId { get; set; }
        public TransportDirection Direction { get; set; }

        public virtual Route Route { get; set; }

        public virtual ICollection<DailyTransportRoute> DailyTransportRoutes { get; set; } = new HashSet<DailyTransportRoute>();
    }
}
