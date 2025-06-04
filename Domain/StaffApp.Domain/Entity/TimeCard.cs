using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class TimeCard : BaseEntity
    {
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public TimeCardStatus Status { get; set; } = TimeCardStatus.Pending;
        public string? ManagerComment { get; set; }

        public virtual ApplicationUser Employee { get; set; }

        public virtual ICollection<TimeCardEntry> TimeCardEntries { get; set; } = new HashSet<TimeCardEntry>();
    }
}
