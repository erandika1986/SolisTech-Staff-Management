using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class ExpenseSupportAttachmentConfiguration : IEntityTypeConfiguration<ExpenseSupportAttachment>
    {
        public void Configure(EntityTypeBuilder<ExpenseSupportAttachment> builder)
        {
            builder.ToTable("ExpenseSupportAttachment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();

            builder
               .HasOne<Expense>(c => c.Expense)
                   .WithMany(c => c.ExpenseSupportAttachments)
                   .HasForeignKey(c => c.ExpenseId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(true);

            builder
               .HasOne<SupportAttachment>(c => c.SupportAttachment)
                   .WithMany(c => c.ExpenseSupportAttachments)
                   .HasForeignKey(c => c.SupportAttachmentId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(true);
        }
    }
}
