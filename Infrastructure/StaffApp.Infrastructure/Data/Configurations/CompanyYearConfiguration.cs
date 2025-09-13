using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class CompanyYearConfiguration : IEntityTypeConfiguration<CompanyYear>
    {
        public void Configure(EntityTypeBuilder<CompanyYear> builder)
        {
            builder.ToTable("CompanyYear");

            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.Year).IsUnique();

            builder.Property(p => p.IsCurrentYear).HasDefaultValue(false).IsRequired(true);

            builder.Property(p => p.Id).ValueGeneratedNever();
        }
    }
}
