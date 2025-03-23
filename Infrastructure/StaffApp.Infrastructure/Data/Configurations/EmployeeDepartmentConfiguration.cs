using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffApp.Domain.Entity;

namespace StaffApp.Infrastructure.Data.Configurations
{
    internal class EmployeeDepartmentConfiguration : IEntityTypeConfiguration<EmployeeDepartment>
    {
        public void Configure(EntityTypeBuilder<EmployeeDepartment> builder)
        {
            builder.ToTable("EmployeeDepartment");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            //builder
            //   .HasOne<ApplicationUser>(c => c.ApplicationUser)
            //   .WithMany(c => c.EmployeeDepartments)
            //   .HasForeignKey(c => c.UserId)
            //   .OnDelete(DeleteBehavior.Cascade)
            //   .IsRequired(true);

            builder
               .HasOne<Department>(c => c.Department)
               .WithMany(c => c.EmployeeDepartments)
               .HasForeignKey(c => c.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(true);

            //builder
            //   .HasOne<ApplicationUser>(c => c.CreatedByUser)
            //   .WithMany(c => c.CreatedEmployeeDepartments)
            //   .HasForeignKey(c => c.CreatedByUserId)
            //   .OnDelete(DeleteBehavior.Restrict)
            //   .IsRequired(true);

            //builder
            //   .HasOne<ApplicationUser>(c => c.UpdatedByUser)
            //   .WithMany(c => c.UpdatedEmployeeDepartments)
            //   .HasForeignKey(c => c.UpdatedByUserId)
            //   .OnDelete(DeleteBehavior.Restrict)
            //   .IsRequired(true);
        }
    }
}
