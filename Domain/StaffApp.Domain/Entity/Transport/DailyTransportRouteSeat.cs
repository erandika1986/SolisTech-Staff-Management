using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity.Transport
{
    public class DailyTransportRouteSeat : BaseAuditableEntity
    {
        public int DailyTransportRouteId { get; set; }
        public string EmployeeId { get; set; }
        public string? Note { get; set; }
        public DailyTransportStatus Status { get; set; }
        public string? ApprovedById { get; set; }

        public virtual DailyTransportRoute DailyTransportRoute { get; set; }
        public virtual ApplicationUser Employee { get; set; }
        public virtual ApplicationUser ApprovedBy { get; set; }
    }
}
