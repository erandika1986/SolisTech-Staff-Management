using StaffApp.Domain.Entity.Common;

namespace StaffApp.Domain.Entity.Master
{
    public class VehiclePurpose : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<VehicleAssignedPurpose> VehicleAssignedPurposes { get; set; } = new HashSet<VehicleAssignedPurpose>();
    }
}
