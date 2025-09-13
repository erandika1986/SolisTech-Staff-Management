using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Leave;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
    {
        public void Configure(EntityTypeBuilder<LeaveType> builder)
        {
            builder.ToTable("LeaveType");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(150);

            builder.Property(p => p.DefaultDays).IsRequired(true);

            builder.Property(p => p.HasLeaveTypeLogic).HasDefaultValue(false).IsRequired(true);

            builder.HasIndex(p => p.Name).IsUnique();

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.IsActive).HasDefaultValue(true).IsRequired(true);
        }
    }
}
