using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPLOYEE.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermissions : Migration
    {
        /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                // 1️⃣ إضافة الصلاحيات
                migrationBuilder.InsertData(
                    table: "Permissions",
                    columns: new[] { "Name" },
                    values: new object[,]
                    {
                    { "Permissions.Evaluations.Manage" },
                    { "Permissions.Evaluations.View" },
                    { "Permissions.Employees.Manage" }
                    });

                // 2️⃣ ربط الصلاحيات بالرول الموجودة
                // نفترض ان Admin, Manager, Employee موجودة مسبقًا
                migrationBuilder.Sql(@"
                DECLARE @AdminId nvarchar(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');
                DECLARE @ManagerId nvarchar(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Manager');
                DECLARE @EmployeeId nvarchar(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Employee');

                DECLARE @ManageEvalId int = (SELECT Id FROM Permissions WHERE Name = 'Permissions.Evaluations.Manage');
                DECLARE @ViewEvalId int = (SELECT Id FROM Permissions WHERE Name = 'Permissions.Evaluations.View');
                DECLARE @ManageEmpId int = (SELECT Id FROM Permissions WHERE Name = 'Permissions.Employees.Manage');

                -- Admin يحصل على كل الصلاحيات
                INSERT INTO RolePermissions (RoleId, PermissionId)
                VALUES (@AdminId, @ManageEvalId), (@AdminId, @ViewEvalId), (@AdminId, @ManageEmpId);

                -- Manager يحصل على صلاحيات التقييم فقط
                INSERT INTO RolePermissions (RoleId, PermissionId)
                VALUES (@ManagerId, @ManageEvalId), (@ManagerId, @ViewEvalId);

                -- Employee يحصل على صلاحية عرض التقييم فقط
                INSERT INTO RolePermissions (RoleId, PermissionId)
                VALUES (@EmployeeId, @ViewEvalId);
            ");
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                // إزالة RolePermissions المرتبطة بهذه الصلاحيات
                migrationBuilder.Sql(@"
                DELETE RP
                FROM RolePermissions RP
                JOIN Permissions P ON RP.PermissionId = P.Id
                WHERE P.Name IN (
                    'Permissions.Evaluations.Manage',
                    'Permissions.Evaluations.View',
                    'Permissions.Employees.Manage'
                );
            ");

                // إزالة الصلاحيات نفسها
                migrationBuilder.Sql(@"
                DELETE FROM Permissions
                WHERE Name IN (
                    'Permissions.Evaluations.Manage',
                    'Permissions.Evaluations.View',
                    'Permissions.Employees.Manage'
                );
            ");
            }
        }
}
