using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class AppraisalPeriodConfiguration : IEntityTypeConfiguration<AppraisalPeriod>
    {
        public void Configure(EntityTypeBuilder<AppraisalPeriod> builder)
        {
            builder.ToTable("AppraisalPeriod");
        }
    }

}
