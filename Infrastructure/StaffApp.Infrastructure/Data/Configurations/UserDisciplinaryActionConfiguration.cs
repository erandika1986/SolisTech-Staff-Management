using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class UserDisciplinaryActionConfiguration : IEntityTypeConfiguration<UserDisciplinaryAction>
    {
        public void Configure(EntityTypeBuilder<UserDisciplinaryAction> builder)
        {
            builder.ToTable("UserDisciplinaryAction");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();


            builder.Property(p => p.Remarks)
                .IsRequired(false);

            builder.Property(p => p.EffectiveUntil)
                .IsRequired(false);

            builder
               .HasOne<ApplicationUser>(c => c.User)
               .WithMany(c => c.UserDisciplinaryActions)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
