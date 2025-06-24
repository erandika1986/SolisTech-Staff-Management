using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;
using StaffApp.Domain.Entity.Authentication;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
        {
            builder.ToTable("InvoiceDetail");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder
               .HasOne<Invoice>(c => c.Invoice)
               .WithMany(c => c.InvoiceDetails)
               .HasForeignKey(c => c.InvoiceId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            builder
               .HasOne<ApplicationUser>(c => c.Employee)
               .WithMany(c => c.InvoiceDetails)
               .HasForeignKey(c => c.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);
        }
    }
}
