using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity.Master;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class VehiclePurposeConfiguration : IEntityTypeConfiguration<VehiclePurpose>
    {
        public void Configure(EntityTypeBuilder<VehiclePurpose> builder)
        {
            builder.ToTable("VehiclePurpose");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
