using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Salary;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeMonthlySalaryConfiguration : IEntityTypeConfiguration<EmployeeMonthlySalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeMonthlySalary> builder)
        {
            builder.ToTable("EmployeeMonthlySalary");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.IsRevised)
               .HasDefaultValue(false)
               .IsRequired(true);

            builder
               .HasOne<MonthlySalary>(c => c.MonthlySalary)
               .WithMany(c => c.EmployeeMonthlySalaries)
               .HasForeignKey(c => c.MonthlySalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<EmployeeSalary>(c => c.EmployeeSalary)
               .WithMany(c => c.EmployeeMonthlySalaries)
               .HasForeignKey(c => c.EmployeeSalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
