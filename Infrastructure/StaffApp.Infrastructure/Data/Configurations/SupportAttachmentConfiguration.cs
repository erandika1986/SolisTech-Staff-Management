using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class SupportAttachmentConfiguration : IEntityTypeConfiguration<SupportAttachment>
    {
        public void Configure(EntityTypeBuilder<SupportAttachment> builder)
        {
            builder.ToTable("SupportAttachment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();
        }
    }
}
