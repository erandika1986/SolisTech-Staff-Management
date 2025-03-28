using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveRequestConfiguration : IEntityTypeConfiguration<EmployeeLeaveRequest>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveRequest> builder)
        {
            builder.ToTable("EmployeeLeaveRequest");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<ApplicationUser>(c => c.Employee)
               .WithMany(c => c.EmployeeLeaveRequests)
               .HasForeignKey(c => c.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<LeaveType>(c => c.LeaveType)
               .WithMany(c => c.EmployeeLeaveRequests)
               .HasForeignKey(c => c.LeaveTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);


            builder.Property(p => p.CreatedDate).IsRequired(true);

            builder.Property(p => p.CreatedByUserId).HasMaxLength(450).IsRequired(true);

            builder.Property(p => p.UpdateDate).IsRequired(true);

            builder.Property(p => p.UpdatedByUserId).HasMaxLength(450).IsRequired(true);

            builder.Property(p => p.IsActive).HasDefaultValue(true).IsRequired(true);
        }
    }
}
