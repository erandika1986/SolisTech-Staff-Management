using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveRequestCommentConfiguration : IEntityTypeConfiguration<EmployeeLeaveRequestComment>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveRequestComment> builder)
        {
            builder.ToTable("EmployeeLeaveRequestComment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Status).IsRequired(true);

            builder
               .HasOne<EmployeeLeaveRequest>(c => c.EmployeeLeaveRequest)
               .WithMany(c => c.EmployeeLeaveRequestComments)
               .HasForeignKey(c => c.EmployeeLeaveRequestId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.CreatedByUser)
               .WithMany(c => c.CreatedEmployeeLeaveRequestComments)
               .HasForeignKey(c => c.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.UpdatedByUser)
               .WithMany(c => c.UpdatedEmployeeLeaveRequestComments)
               .HasForeignKey(c => c.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
