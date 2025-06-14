using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class IncomeTypeConfiguration : IEntityTypeConfiguration<IncomeType>
    {
        public void Configure(EntityTypeBuilder<IncomeType> builder)
        {
            builder.ToTable("IncomeType");

            builder.HasKey(p => p.Id);
        }
    }
}
