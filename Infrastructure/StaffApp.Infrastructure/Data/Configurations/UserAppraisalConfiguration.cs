using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Appraisal;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class UserAppraisalConfiguration : IEntityTypeConfiguration<UserAppraisal>
    {
        public void Configure(EntityTypeBuilder<UserAppraisal> builder)
        {
            builder.ToTable("UserAppraisal");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.AreaForDevelopment)
                .IsRequired(false);

            builder.Property(p => p.GoalsForNextPeriod)
                .IsRequired(false);

            builder
               .HasOne<AppraisalPeriod>(c => c.AppraisalPeriod)
               .WithMany(c => c.UserAppraisals)
               .HasForeignKey(c => c.AppraisalPeriodId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.User)
               .WithMany(c => c.UserAppraisals)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.Reviewer)
               .WithMany(c => c.ReviewAppraisals)
               .HasForeignKey(c => c.ReviewerId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
        }
    }
}
