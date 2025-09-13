using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Leave;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class LeaveTypeConfigConfiguration : IEntityTypeConfiguration<LeaveTypeConfig>
    {
        public void Configure(EntityTypeBuilder<LeaveTypeConfig> builder)
        {
            builder.ToTable("LeaveTypeConfig");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.AnnualLeaveAllowance).HasColumnType("decimal(5, 2)");

            builder
               .HasOne<EmployeeType>(c => c.EmployeeType)
               .WithMany(c => c.LeaveTypeConfigurations)
               .HasForeignKey(c => c.EmployeeTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<LeaveType>(c => c.LeaveType)
               .WithMany(c => c.LeaveTypeConfigurations)
               .HasForeignKey(c => c.LeaveTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
