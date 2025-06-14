using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expense");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();

            builder
               .HasOne<ExpenseType>(c => c.ExpenseType)
               .WithMany(c => c.Expenses)
               .HasForeignKey(c => c.ExpenseTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
