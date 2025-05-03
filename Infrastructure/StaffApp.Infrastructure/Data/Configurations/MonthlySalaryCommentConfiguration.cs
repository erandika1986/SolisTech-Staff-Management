using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class MonthlySalaryCommentConfiguration : IEntityTypeConfiguration<MonthlySalaryComment>
    {
        public void Configure(EntityTypeBuilder<MonthlySalaryComment> builder)
        {
            builder.ToTable("MonthlySalaryComment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<MonthlySalary>(c => c.MonthlySalary)
               .WithMany(c => c.MonthlySalaryComments)
               .HasForeignKey(c => c.MonthlySalaryId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
