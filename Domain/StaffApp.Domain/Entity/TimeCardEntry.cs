using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity
{
    public class TimeCardEntry : BaseEntity
    {
        public int TimeCardId { get; set; }
        public int ProjectId { get; set; }

        public double HoursWorked { get; set; }
        public string Notes { get; set; }

        public virtual TimeCard TimeCard { get; set; }
        public virtual Project Project { get; set; }
    }
}
