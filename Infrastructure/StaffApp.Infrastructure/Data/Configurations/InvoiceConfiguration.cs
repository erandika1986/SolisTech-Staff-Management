﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoice");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property(p => p.TotalHours).IsRequired(false);

            builder.Property(p => p.Status)
                   .HasDefaultValue(StaffApp.Domain.Enum.InvoiceStatus.Draft)
                   .IsRequired(true);

            builder
               .HasOne<Project>(c => c.Project)
               .WithMany(c => c.Invoices)
               .HasForeignKey(c => c.ProjectId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
        }
    }
}
