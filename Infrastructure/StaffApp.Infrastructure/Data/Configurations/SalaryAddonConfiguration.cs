using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class SalaryAddonConfiguration : IEntityTypeConfiguration<SalaryAddon>
    {
        public void Configure(EntityTypeBuilder<SalaryAddon> builder)
        {
            builder.ToTable("SalaryAddon");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(250);

            builder.Property(p => p.ApplyForAllEmployees)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true)
                .IsRequired(true);

            builder.Property(p => p.CreatedDate)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired(true);

        }
    }
}
