using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Transport;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class DailyTransportRouteSeatConfiguration : IEntityTypeConfiguration<DailyTransportRouteSeat>
    {
        public void Configure(EntityTypeBuilder<DailyTransportRouteSeat> builder)
        {
            builder.ToTable("DailyTransportRouteSeat");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<DailyTransportRoute>(c => c.DailyTransportRoute)
               .WithMany(c => c.DailyTransportRouteSeats)
               .HasForeignKey(c => c.DailyTransportRouteId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.Employee)
               .WithMany(c => c.EmployeeDailyTransportRouteSeats)
               .HasForeignKey(c => c.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.ApprovedBy)
               .WithMany(c => c.ApprovedDailyTransportRouteSeats)
               .HasForeignKey(c => c.ApprovedById)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
        }
    }
}
