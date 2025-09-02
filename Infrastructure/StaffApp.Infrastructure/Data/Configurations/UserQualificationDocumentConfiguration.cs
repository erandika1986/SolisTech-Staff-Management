using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class UserQualificationDocumentConfiguration : IEntityTypeConfiguration<UserQualificationDocument>
    {
        public void Configure(EntityTypeBuilder<UserQualificationDocument> builder)
        {
            builder.ToTable("UserQualificationDocument");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
               .HasOne<ApplicationUser>(c => c.User)
               .WithMany(c => c.UserQualificationDocuments)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
