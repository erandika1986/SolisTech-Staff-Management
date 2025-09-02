using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class UserAppraisalCriteriaConfiguration : IEntityTypeConfiguration<UserAppraisalCriteria>
    {
        public void Configure(EntityTypeBuilder<UserAppraisalCriteria> builder)
        {
            builder.ToTable("UserAppraisalCriteria");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
