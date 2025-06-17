using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class IncomeConfiguration : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.ToTable("Income");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<IncomeType>(c => c.IncomeType)
               .WithMany(c => c.Incomes)
               .HasForeignKey(c => c.IncomeTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
