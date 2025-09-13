using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Transport
{
    public class Route : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string PickupPoint { get; set; }

        public virtual ICollection<PredefineRoute> PredefineRoutes { get; set; } = new HashSet<PredefineRoute>();
    }
}
