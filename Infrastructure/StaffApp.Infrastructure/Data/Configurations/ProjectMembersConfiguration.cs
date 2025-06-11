using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class ProjectMembersConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.ToTable("ProjectMember");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
               .HasOne<Project>(c => c.Project)
               .WithMany(c => c.ProjectMembers)
               .HasForeignKey(c => c.ProjectId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.Member)
               .WithMany(c => c.ProjectMembers)
               .HasForeignKey(c => c.MemberId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            //builder
            //   .HasOne<ApplicationRole>(c => c.Role)
            //   .WithMany(c => c.ProjectMembers)
            //   .HasForeignKey(c => c.RoleId)
            //   .OnDelete(DeleteBehavior.Restrict)
            //   .IsRequired(true);
        }
    }
}
