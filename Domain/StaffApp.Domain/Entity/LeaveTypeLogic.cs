using StaffApp.Domain.Entity.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffApp.Domain.Entity
{
    public class LeaveTypeLogic : BaseEntity
    {
        [Column(Order = 1)]
        public int LeaveTypeId { get; set; }

        [Column(Order = 2)]
        public int StartMonthOfYear { get; set; }

        [Column(Order = 3)]
        public int StartDateOfMonth { get; set; }

        [Column(Order = 4)]
        public int EndMonthOfYear { get; set; }

        [Column(Order = 5)]
        public int EndDateOfMonth { get; set; }

        [Column(Order = 6)]
        public decimal EntitledLeaveCount { get; set; }

        public virtual LeaveType LeaveType { get; set; }
    }
}
