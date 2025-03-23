using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryConfiguration : IEntityTypeConfiguration<EmployeeSalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalary> builder)
        {
            builder.ToTable("EmployeeSalary");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<ApplicationUser>(c => c.User)
               .WithMany(c => c.EmployeeSalaries)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.CreatedByUser)
               .WithMany(c => c.CreatedEmployeeSalaries)
               .HasForeignKey(c => c.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.UpdatedByUser)
               .WithMany(c => c.UpdatedEmployeeSalaries)
               .HasForeignKey(c => c.UpdatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
