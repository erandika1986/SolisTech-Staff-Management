using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class LeaveTypeLogicConfiguration : IEntityTypeConfiguration<LeaveTypeLogic>
    {
        public void Configure(EntityTypeBuilder<LeaveTypeLogic> builder)
        {
            builder.ToTable("LeaveTypeLogic");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<LeaveType>(c => c.LeaveType)
               .WithMany(c => c.LeaveTypeLogics)
               .HasForeignKey(c => c.LeaveTypeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);
        }
    }
}
