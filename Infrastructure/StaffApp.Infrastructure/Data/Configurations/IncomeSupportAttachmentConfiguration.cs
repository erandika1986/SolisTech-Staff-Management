using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class IncomeSupportAttachmentConfiguration : IEntityTypeConfiguration<IncomeSupportAttachment>
    {
        public void Configure(EntityTypeBuilder<IncomeSupportAttachment> builder)
        {
            builder.ToTable("IncomeSupportAttachment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();

            builder
               .HasOne<Income>(c => c.Income)
                       .WithMany(c => c.IncomeSupportAttachments)
                       .HasForeignKey(c => c.IncomeId)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired(true);

            builder
               .HasOne<SupportAttachment>(c => c.SupportAttachment)
                       .WithMany(c => c.IncomeSupportAttachments)
                       .HasForeignKey(c => c.SupportAttachmentId)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired(true);
        }
    }
}
