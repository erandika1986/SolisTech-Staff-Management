using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class DocumentNameConfiguration : IEntityTypeConfiguration<DocumentName>
    {
        public void Configure(EntityTypeBuilder<DocumentName> builder)
        {
            builder.ToTable("DocumentName");

            builder.HasKey(p => p.Id);
        }
    }
}
