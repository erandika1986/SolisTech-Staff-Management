using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Salary;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class TaxLogicConfiguration : IEntityTypeConfiguration<TaxLogic>
    {
        public void Configure(EntityTypeBuilder<TaxLogic> builder)
        {
            builder.ToTable("TaxLogic");

            builder.HasKey(p => p.Id);

            builder
               .HasOne<SalaryAddon>(c => c.SalaryAddon)
               .WithMany(c => c.TaxLogics)
               .HasForeignKey(c => c.SalaryAddonId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
