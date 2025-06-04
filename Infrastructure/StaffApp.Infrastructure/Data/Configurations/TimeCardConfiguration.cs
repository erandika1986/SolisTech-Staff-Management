using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class TimeCardConfiguration : IEntityTypeConfiguration<TimeCard>
    {
        public void Configure(EntityTypeBuilder<TimeCard> builder)
        {
            builder.ToTable("TimeCard");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
           .HasOne<ApplicationUser>(c => c.Employee)
           .WithMany(c => c.TimeCards)
           .HasForeignKey(c => c.EmployeeID)
           .OnDelete(DeleteBehavior.Restrict)
           .IsRequired(true);
        }
    }
}
