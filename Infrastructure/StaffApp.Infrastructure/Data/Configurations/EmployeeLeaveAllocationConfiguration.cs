using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveAllocationConfiguration : IEntityTypeConfiguration<EmployeeLeaveAllocation>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveAllocation> builder)
        {
            builder.ToTable("EmployeeLeaveAllocation");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.AllocatedLeaveCount).HasColumnType("decimal(5, 2)");

            builder.Property(p => p.RemainingLeaveCount).HasColumnType("decimal(5, 2)");


            builder.Property(p => p.CreatedDate).IsRequired(true);

            builder.Property(p => p.CreatedByUserId).HasMaxLength(450).IsRequired(true);

            builder.Property(p => p.UpdateDate).IsRequired(true);

            builder.Property(p => p.UpdatedByUserId).HasMaxLength(450).IsRequired(true);

            builder.Property(p => p.IsActive).HasDefaultValue(true).IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.Employee)
               .WithMany(c => c.EmployeeLeaveBalances)
               .HasForeignKey(c => c.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<LeaveType>(c => c.LeaveType)
               .WithMany(c => c.EmployeeLeaveBalances)
               .HasForeignKey(c => c.LeaveTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<CompanyYear>(c => c.CompanyYear)
               .WithMany(c => c.EmployeeLeaveBalances)
               .HasForeignKey(c => c.CompanyYearId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            //builder
            //   .HasOne<ApplicationUser>(c => c.CreatedByUser)
            //   .WithMany(c => c.CreatedEmployeeLeaveBalances)
            //   .HasForeignKey(c => c.CreatedByUserId)
            //   .OnDelete(DeleteBehavior.Restrict)
            //   .IsRequired(true);

            //builder
            //   .HasOne<ApplicationUser>(c => c.UpdatedByUser)
            //   .WithMany(c => c.UpdatedEmployeeLeaveBalances)
            //   .HasForeignKey(c => c.UpdatedByUserId)
            //   .OnDelete(DeleteBehavior.Restrict)
            //   .IsRequired(true);
        }
    }
}
