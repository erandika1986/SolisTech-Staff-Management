using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class MonthlySalaryConfiguration : IEntityTypeConfiguration<MonthlySalary>
    {
        public void Configure(EntityTypeBuilder<MonthlySalary> builder)
        {
            builder.ToTable("MonthlySalary");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<CompanyYear>(c => c.CompanyYear)
               .WithMany(c => c.MonthlySalaries)
               .HasForeignKey(c => c.CompanyYearId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
