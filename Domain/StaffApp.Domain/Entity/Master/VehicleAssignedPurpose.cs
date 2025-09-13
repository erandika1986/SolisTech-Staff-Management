namespace StaffApp.Domain.Entity.Master
{
    public class VehicleAssignedPurpose
    {
        public int VehiclePurposeId { get; set; }
        public int VehicleId { get; set; }

        public virtual VehiclePurpose VehiclePurpose { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
