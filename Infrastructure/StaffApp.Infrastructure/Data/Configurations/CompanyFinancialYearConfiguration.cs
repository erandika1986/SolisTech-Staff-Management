using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class CompanyFinancialYearConfiguration : IEntityTypeConfiguration<CompanyFinancialYear>
    {
        public void Configure(EntityTypeBuilder<CompanyFinancialYear> builder)
        {
            builder.ToTable("CompanyFinancialYear");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Year).IsUnique();

            builder.Property(p => p.IsCurrentYear).HasDefaultValue(false).IsRequired(true);

            builder.Property(p => p.Id).ValueGeneratedNever();
        }
    }
}
