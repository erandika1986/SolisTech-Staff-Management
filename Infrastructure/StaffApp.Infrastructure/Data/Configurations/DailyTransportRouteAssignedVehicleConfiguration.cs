using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Master;
using StaffApp.Domain.Entity.Transport;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class DailyTransportRouteAssignedVehicleConfiguration : IEntityTypeConfiguration<DailyTransportRouteAssignedVehicle>
    {
        public void Configure(EntityTypeBuilder<DailyTransportRouteAssignedVehicle> builder)
        {
            builder.ToTable("DailyTransportRouteAssignedVehicle");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<DailyTransportRoute>(c => c.DailyTransportRoute)
               .WithMany(c => c.DailyTransportRouteAssignedVehicles)
               .HasForeignKey(c => c.DailyTransportRouteId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<Vehicle>(c => c.Vehicle)
               .WithMany(c => c.DailyTransportRouteAssignedVehicles)
               .HasForeignKey(c => c.VehicleId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
