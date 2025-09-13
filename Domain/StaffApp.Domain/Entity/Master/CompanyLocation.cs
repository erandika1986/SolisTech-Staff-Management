using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Master
{
    public class CompanyLocation : BaseEntity
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string TimeZone { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
