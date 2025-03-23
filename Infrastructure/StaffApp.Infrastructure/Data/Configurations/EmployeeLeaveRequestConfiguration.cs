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

            builder
               .HasOne<ApplicationUser>(c => c.CreatedByUser)
               .WithMany(c => c.CreatedEmployeeLeaveRequests)
               .HasForeignKey(c => c.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.UpdatedByUser)
               .WithMany(c => c.UpdatedEmployeeLeaveRequests)
               .HasForeignKey(c => c.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
