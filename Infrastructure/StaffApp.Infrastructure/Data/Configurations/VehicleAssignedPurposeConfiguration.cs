using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class VehicleAssignedPurposeConfiguration : IEntityTypeConfiguration<VehicleAssignedPurpose>
    {
        public void Configure(EntityTypeBuilder<VehicleAssignedPurpose> builder)
        {
            builder.ToTable("VehicleAssignedPurpose");

            builder.HasKey(p => new { p.VehicleId, p.VehiclePurposeId });

            builder
               .HasOne<Vehicle>(c => c.Vehicle)
               .WithMany(c => c.VehicleAssignedPurposes)
               .HasForeignKey(c => c.VehicleId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<VehiclePurpose>(c => c.VehiclePurpose)
               .WithMany(c => c.VehicleAssignedPurposes)
               .HasForeignKey(c => c.VehiclePurposeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

        }
    }
}
