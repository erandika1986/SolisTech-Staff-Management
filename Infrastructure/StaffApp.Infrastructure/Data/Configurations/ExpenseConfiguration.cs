using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expense");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.CompanyShare).IsRequired(false);

            builder.Property(p => p.EmployeeShare).IsRequired(false);

            builder
               .HasOne<CompanyYear>(c => c.CompanyYear)
               .WithMany(c => c.Expenses)
               .HasForeignKey(c => c.CompanyYearId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ExpenseType>(c => c.ExpenseType)
               .WithMany(c => c.Expenses)
               .HasForeignKey(c => c.ExpenseTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
