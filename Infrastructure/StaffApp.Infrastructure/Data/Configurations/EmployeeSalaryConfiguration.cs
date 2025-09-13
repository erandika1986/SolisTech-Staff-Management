using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Authentication;
using StaffApp.Domain.Entity.Salary;
using StaffApp.Domain.Enum;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryConfiguration : IEntityTypeConfiguration<EmployeeSalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalary> builder)
        {
            builder.ToTable("EmployeeSalary");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.Status)
                .HasDefaultValue(EmployeeSalaryStatus.SubmittedForApproval)
                .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.User)
               .WithMany(c => c.EmployeeSalaries)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
