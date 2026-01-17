using EMPLOYEE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMPLOYEE.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.ManagerUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(d => d.ManagerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
