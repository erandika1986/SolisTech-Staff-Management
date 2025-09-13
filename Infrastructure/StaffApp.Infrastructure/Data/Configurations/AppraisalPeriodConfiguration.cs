using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Appraisal;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class AppraisalPeriodConfiguration : IEntityTypeConfiguration<AppraisalPeriod>
    {
        public void Configure(EntityTypeBuilder<AppraisalPeriod> builder)
        {
            builder.ToTable("AppraisalPeriod");

            builder
               .HasOne<CompanyYear>(c => c.CompanyYear)
               .WithMany(c => c.AppraisalPeriods)
               .HasForeignKey(c => c.CompanyYearId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }

}
