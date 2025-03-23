using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).HasMaxLength(150);

            builder.Property(p => p.DepartmentHeadId).IsRequired(false);

            builder.HasIndex(p => p.Name).IsUnique();

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.CreatedDate).IsRequired(false);

            builder.Property(p => p.CreatedByUserId).IsRequired(false);

            builder.Property(p => p.UpdateDate).IsRequired(false);

            builder.Property(p => p.UpdatedByUserId).IsRequired(false);

            builder.Property(p => p.IsActive).HasDefaultValue(true).IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.DepartmentHead)
               .WithMany(c => c.DepartmentHeads)
               .HasForeignKey(c => c.DepartmentHeadId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
