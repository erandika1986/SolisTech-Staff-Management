using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Transport;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class DailyTransportRouteConfiguration : IEntityTypeConfiguration<DailyTransportRoute>
    {
        public void Configure(EntityTypeBuilder<DailyTransportRoute> builder)
        {
            builder.ToTable("DailyTransportRoute");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<PredefineRoute>(c => c.PredefineRoute)
               .WithMany(c => c.DailyTransportRoutes)
               .HasForeignKey(c => c.PredefineRouteId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
