using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    public class EmployeeBankAccountConfiguration : IEntityTypeConfiguration<EmployeeBankAccount>
    {
        public void Configure(EntityTypeBuilder<EmployeeBankAccount> builder)
        {
            builder.ToTable("EmployeeBankAccount");

            builder.HasKey(p => p.Id);

            builder
               .HasOne<ApplicationUser>(c => c.Employee)
               .WithMany(c => c.EmployeeBankAccounts)
               .HasForeignKey(c => c.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

        }
    }
}
