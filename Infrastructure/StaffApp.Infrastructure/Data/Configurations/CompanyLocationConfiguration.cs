using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class CompanyLocationConfiguration : IEntityTypeConfiguration<CompanyLocation>
    {
        public void Configure(EntityTypeBuilder<CompanyLocation> builder)
        {
            builder.ToTable("CompanyLocation");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
