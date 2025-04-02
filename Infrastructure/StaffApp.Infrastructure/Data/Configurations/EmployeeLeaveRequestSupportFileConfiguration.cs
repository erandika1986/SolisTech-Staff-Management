using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveRequestSupportFileConfiguration : IEntityTypeConfiguration<EmployeeLeaveRequestSupportFile>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveRequestSupportFile> builder)
        {
            builder.ToTable("EmployeeLeaveRequestSupportFile");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<EmployeeLeaveRequest>(c => c.EmployeeLeaveRequest)
               .WithMany(c => c.EmployeeLeaveRequestSupportFiles)
               .HasForeignKey(c => c.EmployeeLeaveRequestId)
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
