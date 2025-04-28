using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeMonthlySalaryConfiguration : IEntityTypeConfiguration<EmployeeMonthlySalary>
    {
        public void Configure(EntityTypeBuilder<EmployeeMonthlySalary> builder)
        {
            builder.ToTable("EmployeeMonthlySalary");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<CompanyYear>(c => c.CompnayYear)
               .WithMany(c => c.EmployeeMonthlySalaries)
               .HasForeignKey(c => c.CompanyYearId)
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
