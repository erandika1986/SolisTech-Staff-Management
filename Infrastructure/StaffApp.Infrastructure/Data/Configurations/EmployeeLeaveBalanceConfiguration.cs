using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeLeaveBalanceConfiguration : IEntityTypeConfiguration<EmployeeLeaveBalance>
    {
        public void Configure(EntityTypeBuilder<EmployeeLeaveBalance> builder)
        {
            builder.ToTable("EmployeeLeaveBalance");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

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

            builder
               .HasOne<ApplicationUser>(c => c.CreatedByUser)
               .WithMany(c => c.CreatedEmployeeLeaveBalances)
               .HasForeignKey(c => c.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.UpdatedByUser)
               .WithMany(c => c.UpdatedEmployeeLeaveBalances)
               .HasForeignKey(c => c.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
