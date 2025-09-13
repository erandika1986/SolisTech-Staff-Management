using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Transport;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class PredefineRouteConfiguration : IEntityTypeConfiguration<PredefineRoute>
    {
        public void Configure(EntityTypeBuilder<PredefineRoute> builder)
        {
            builder.ToTable("PredefineRoute");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<Route>(c => c.Route)
               .WithMany(c => c.PredefineRoutes)
               .HasForeignKey(c => c.RouteId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
