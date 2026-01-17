using EMPLOYEE.Application.Common.Authorization;
using EMPLOYEE.Migrations;
using EMPLOYEE.Models;
using EMPLOYEE.Setting;
using EmployeeDomain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using EmployeeEvaluation = EmployeeDomain.Entities.EmployeeEvaluation;
namespace EMPLOYEE.Data { 
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            builder.Entity<Employee>()
     .HasQueryFilter(e => e.IsActive);

            builder.Entity<EmailSettings>(b =>
            {
                b.ToTable("EmailSettings");
                b.Property(e => e.Host).HasDefaultValue("smtp.gmail.com");
                b.Property(e => e.Port).HasDefaultValue(465);
                b.Property(e => e.EnableSSL).HasDefaultValue(true);
                b.Property(e => e.Email).HasDefaultValue(string.Empty);
                b.Property(e => e.Password).HasDefaultValue(string.Empty);
            });
            builder.Entity<Employee>(e=>e.Property(e=>e.IsActive).HasDefaultValue(true));
            builder.Entity<RolePermission>()
           .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        }

        public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
    public DbSet<EmployeeEvaluation> EmployeeEvaluations => Set<EmployeeEvaluation>();
    public DbSet<EmailSettings> EmailSetting { get; set; }
    public DbSet<Permissions> Permissions => Set<Permissions>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();


    }
}