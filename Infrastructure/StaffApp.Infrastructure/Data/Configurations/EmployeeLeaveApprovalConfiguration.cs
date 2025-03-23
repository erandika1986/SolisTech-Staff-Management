using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveApprovalConfiguration : IEntityTypeConfiguration<EmployeeLeaveApproval>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveApproval> builder)
        {
            builder.ToTable("EmployeeLeaveApproval");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<EmployeeLeaveRequest>(c => c.EmployeeLeaveRequest)
               .WithMany(c => c.EmployeeLeaveApprovals)
               .HasForeignKey(c => c.EmployeeLeaveRequestId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.CreatedByUser)
               .WithMany(c => c.CreatedEmployeeLeaveApprovals)
               .HasForeignKey(c => c.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.UpdatedByUser)
               .WithMany(c => c.UpdatedEmployeeLeaveApprovals)
               .HasForeignKey(c => c.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
