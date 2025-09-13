using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Salary;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeMonthlySalaryAddonConfiguration : IEntityTypeConfiguration<EmployeeMonthlySalaryAddon>
    {
        public void Configure(EntityTypeBuilder<EmployeeMonthlySalaryAddon> builder)
        {
            builder.ToTable("EmployeeMonthlySalaryAddon");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.IsPayeApplicable)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder.Property(p => p.ConsiderForSocialSecurityScheme)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder
               .HasOne<EmployeeMonthlySalary>(c => c.EmployeeMonthlySalary)
               .WithMany(c => c.EmployeeMonthlySalaryAddons)
               .HasForeignKey(c => c.EmployeeMonthlySalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<SalaryAddon>(c => c.SalaryAddon)
               .WithMany(c => c.EmployeeMonthlySalaryAddons)
               .HasForeignKey(c => c.SalaryAddonId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
