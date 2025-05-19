using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class ProjectDocumentsConfiguration : IEntityTypeConfiguration<ProjectDocument>
    {
        public void Configure(EntityTypeBuilder<ProjectDocument> builder)
        {
            builder.ToTable("ProjectDocument");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
               .HasOne<Project>(c => c.Project)
               .WithMany(c => c.ProjectDocuments)
               .HasForeignKey(c => c.ProjectId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
