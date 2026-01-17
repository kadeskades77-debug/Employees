namespace EMPLOYEE.Data
{
    using EMPLOYEE.Models;
    using Microsoft.AspNetCore.Identity;

    public static class RoleSeeder
    {
        public static async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // ================================
            // 1. إنشاء Roles إذا لم تكن موجودة
            // ================================
            var roles = new[] { "Manager", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ================================
            // 2. إنشاء Managers
            // ================================
            var managers = new List<ApplicationUser>
        {
            new() { UserName = "manager1@company.com", Email = "manager1@company.com", FirstName = "Ali", LastName = "Saleh" },
            new() { UserName = "manager2@company.com", Email = "manager2@company.com", FirstName = "Sara", LastName = "Ahmed" }
        };

            foreach (var manager in managers)
            {
                var existing = await userManager.FindByEmailAsync(manager.Email);
                if (existing == null)
                {
                    await userManager.CreateAsync(manager, "Password123!");
                    await userManager.AddToRoleAsync(manager, "Manager");
                }
            }

            // ================================
            // 3. إنشاء Employees
            // ================================
            var employees = new List<ApplicationUser>();
            for (int i = 1; i <= 5; i++)
            {
                employees.Add(new ApplicationUser
                {
                    UserName = $"employee{i}@company.com",
                    Email = $"employee{i}@company.com",
                    FirstName = $"Emp{i}",
                    LastName = $"Last{i}"
                });
            }

            foreach (var emp in employees)
            {
                var existing = await userManager.FindByEmailAsync(emp.Email);
                if (existing == null)
                {
                    await userManager.CreateAsync(emp, "Password123!");
                    await userManager.AddToRoleAsync(emp, "Employee");
                }
            }
        }
    }

}
