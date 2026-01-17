using EMPLOYEE.Data;
using EMPLOYEE.Models;
using EmployeeDomain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.Departments.Any())
            return; // تم السِيد مسبقًا

        // ===========================
        // 1. إنشاء المدراء كمستخدمين
        // ===========================
        var managers = new List<ApplicationUser>
        {
            new() { UserName = "manager1@company.com", Email = "manager1@company.com", FirstName = "Ali", LastName = "Saleh" },
            new() { UserName = "manager2@company.com", Email = "manager2@company.com", FirstName = "Sara", LastName = "Ahmed" },
            new() { UserName = "manager3@company.com", Email = "manager3@company.com", FirstName = "Khaled", LastName = "Hassan" },
            new() { UserName = "manager4@company.com", Email = "manager4@company.com", FirstName = "Laila", LastName = "Omar" },
            new() { UserName = "manager5@company.com", Email = "manager5@company.com", FirstName = "Yousef", LastName = "Tariq" }
        };

        foreach (var manager in managers)
        {
            await userManager.CreateAsync(manager, "Password123!");
        }

        // ===========================
        // 2. إنشاء الأقسام
        // ===========================
        var departments = new List<Department>
        {
            new() { Name = "HR", ManagerUserId = managers[0].Id },
            new() { Name = "IT", ManagerUserId = managers[1].Id },
            new() { Name = "Finance", ManagerUserId = managers[2].Id },
            new() { Name = "Marketing", ManagerUserId = managers[3].Id },
            new() { Name = "Sales", ManagerUserId = managers[4].Id },
            new() { Name = "Support", ManagerUserId = managers[1].Id },
            new() { Name = "Logistics", ManagerUserId = managers[2].Id },
            new() { Name = "Procurement", ManagerUserId = managers[3].Id },
            new() { Name = "R&D", ManagerUserId = managers[0].Id },
            new() { Name = "Legal", ManagerUserId = managers[4].Id }
        };

        context.Departments.AddRange(departments);
        await context.SaveChangesAsync();

        // ===========================
        // 3. إنشاء الموظفين كمستخدمين
        // ===========================
        var employeesUsers = new List<ApplicationUser>();
        for (int i = 1; i <= 20; i++)
        {
            employeesUsers.Add(new ApplicationUser
            {
                UserName = $"emp{i}@company.com",
                Email = $"emp{i}@company.com",
                FirstName = $"Emp{i}",
                LastName = $"Last{i}"
            });
        }

        foreach (var empUser in employeesUsers)
        {
            await userManager.CreateAsync(empUser, "Password123!");
        }

        // ===========================
        // 4. إنشاء الموظفين وربطهم بالأقسام
        // ===========================
        var rnd = new Random();
        var employees = employeesUsers.Select((u, index) => new Employee
        {
            UserId = u.Id,
            DepartmentId = departments[rnd.Next(departments.Count)].Id,
            Position = $"Position{index + 1}",
            Salary = rnd.Next(3000, 7000)
        }).ToList();

        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        // ===========================
        // 5. إنشاء تقييمات الموظفين
        // ===========================
        var evaluations = new List<EmployeeEvaluation>();
        foreach (var emp in employees)
        {
            var dept = departments.First(d => d.Id == emp.DepartmentId);
            var managerId = dept.ManagerUserId;

            int evalCount = rnd.Next(3, 6); // 3-5 تقييمات لكل موظف
        //    for (int j = 0; j < evalCount; j++)
        //    {
        //        int attendance = rnd.Next(1, 6);
        //        int quality = rnd.Next(1, 6);
        //        int productivity = rnd.Next(1, 6);
        //        int teamwork = rnd.Next(1, 6);
        //        decimal finalScore = Math.Round((attendance + quality + productivity + teamwork) / 4m, 2);

        //        evaluations.Add(new EmployeeEvaluation
        //        {
        //            EmployeeId = emp.Id,
        //            ManagerId = managerId,
        //            Attendance = attendance,
        //            Quality = quality,
        //            Productivity = productivity,
        //            Teamwork = teamwork,
        //            FinalScore = finalScore,
        //            Comments = $"Evaluation {j + 1} for {emp.UserId}",
        //            Period = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1),

        //            CreatedAt = DateTime.UtcNow.AddMonths(-j)
        //        });
        //    }
        }

        context.EmployeeEvaluations.AddRange(evaluations);
        await context.SaveChangesAsync();
    }
}
