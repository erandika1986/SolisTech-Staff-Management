using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Leave;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class LeaveTypeAllowDurationConfiguration : IEntityTypeConfiguration<LeaveTypeAllowDuration>
    {
        public void Configure(EntityTypeBuilder<LeaveTypeAllowDuration> builder)
        {
            builder.ToTable("LeaveTypeAllowDuration");

            builder.HasKey(p => p.Id);

            builder
               .HasOne<LeaveType>(c => c.LeaveType)
               .WithMany(c => c.LeaveTypeAllowDurations)
               .HasForeignKey(c => c.LeaveTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
