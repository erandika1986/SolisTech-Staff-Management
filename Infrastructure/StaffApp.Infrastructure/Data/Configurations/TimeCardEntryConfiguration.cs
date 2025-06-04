using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class TimeCardEntryConfiguration : IEntityTypeConfiguration<TimeCardEntry>
    {
        public void Configure(EntityTypeBuilder<TimeCardEntry> builder)
        {
            builder.ToTable("TimeCardEntry");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
                .HasOne<TimeCard>(c => c.TimeCard)
                .WithMany(c => c.TimeCardEntries)
                .HasForeignKey(c => c.TimeCardId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);

            builder
                .HasOne<Project>(c => c.Project)
                .WithMany(c => c.TimeCardEntries)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(true);
        }
    }
}
