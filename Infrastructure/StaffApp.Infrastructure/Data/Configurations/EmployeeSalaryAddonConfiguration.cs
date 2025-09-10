using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryAddonConfiguration : IEntityTypeConfiguration<EmployeeSalaryAddon>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryAddon> builder)
        {
            builder.ToTable("EmployeeSalaryAddon");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.ConsiderForSocialSecurityScheme)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder
               .HasOne<EmployeeSalary>(c => c.EmployeeSalary)
               .WithMany(c => c.EmployeeSalaryAddons)
               .HasForeignKey(c => c.EmployeeSalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<SalaryAddon>(c => c.SalaryAddon)
               .WithMany(c => c.EmployeeSalaryAddons)
               .HasForeignKey(c => c.SalaryAddonId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }

}
