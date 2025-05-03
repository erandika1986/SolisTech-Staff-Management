using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeSalaryCommentConfiguration : IEntityTypeConfiguration<EmployeeSalaryComment>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryComment> builder)
        {
            builder.ToTable("EmployeeSalaryComment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<EmployeeSalary>(c => c.EmployeeSalary)
               .WithMany(c => c.EmployeeSalaryComments)
               .HasForeignKey(c => c.EmployeeSalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
