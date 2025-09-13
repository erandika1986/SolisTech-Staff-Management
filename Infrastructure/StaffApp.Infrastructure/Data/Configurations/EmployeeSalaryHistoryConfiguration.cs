using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Salary;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryHistoryConfiguration : IEntityTypeConfiguration<EmployeeSalaryHistory>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryHistory> builder)
        {
            builder.ToTable("EmployeeSalaryHistory");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<EmployeeSalary>(c => c.EmployeeSalary)
               .WithMany(c => c.EmployeeSalaryHistories)
               .HasForeignKey(c => c.EmployeeSalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
