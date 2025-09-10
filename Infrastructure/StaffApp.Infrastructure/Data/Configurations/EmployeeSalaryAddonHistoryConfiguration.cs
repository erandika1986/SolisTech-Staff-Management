using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryAddonHistoryConfiguration : IEntityTypeConfiguration<EmployeeSalaryAddonHistory>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryAddonHistory> builder)
        {
            builder.ToTable("EmployeeSalaryAddonHistory");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.ConsiderForSocialSecurityScheme)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder
               .HasOne<EmployeeSalaryHistory>(c => c.EmployeeSalaryHistory)
               .WithMany(c => c.EmployeeSalaryAddonHistories)
               .HasForeignKey(c => c.EmployeeSalaryHistoryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<EmployeeSalary>(c => c.EmployeeSalary)
               .WithMany(c => c.EmployeeSalaryAddonHistories)
               .HasForeignKey(c => c.EmployeeSalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<SalaryAddon>(c => c.SalaryAddon)
               .WithMany(c => c.EmployeeSalaryAddonHistories)
               .HasForeignKey(c => c.SalaryAddonId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
