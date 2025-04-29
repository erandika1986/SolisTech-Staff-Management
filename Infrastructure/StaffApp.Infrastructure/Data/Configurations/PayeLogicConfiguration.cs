using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class PayeLogicConfiguration : IEntityTypeConfiguration<PayeLogic>
    {
        public void Configure(EntityTypeBuilder<PayeLogic> builder)
        {
            builder.ToTable("PayeLogic");

            builder.HasKey(p => p.Id);
        }
    }
}
