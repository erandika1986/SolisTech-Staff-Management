using StaffApp.Domain.Entity.Common;
using StaffApp.Domain.Enum;

namespace StaffApp.Domain.Entity
{
    public class TimeCardEntry : BaseEntity
    {
        public int TimeCardId { get; set; }
        public int ProjectId { get; set; }
        public double HoursWorked { get; set; }
        public string? Notes { get; set; }
        public string? ManagerComment { get; set; }
        public TimeCardEntryStatus Status { get; set; }

        public virtual TimeCard TimeCard { get; set; }
        public virtual Project Project { get; set; }
    }
}
