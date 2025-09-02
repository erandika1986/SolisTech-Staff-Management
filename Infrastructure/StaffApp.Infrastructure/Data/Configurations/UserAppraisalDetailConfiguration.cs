using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class UserAppraisalDetailConfiguration : IEntityTypeConfiguration<UserAppraisalDetail>
    {
        public void Configure(EntityTypeBuilder<UserAppraisalDetail> builder)
        {
            builder.ToTable("UserAppraisalDetail");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
               .HasOne<UserAppraisal>(c => c.UserAppraisal)
               .WithMany(c => c.UserAppraisalDetails)
               .HasForeignKey(c => c.AppraisalID)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<UserAppraisalCriteria>(c => c.UserAppraisalCriteria)
               .WithMany(c => c.UserAppraisalDetails)
               .HasForeignKey(c => c.CriteriaID)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
