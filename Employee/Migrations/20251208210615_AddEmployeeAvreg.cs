using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPLOYEE.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAvreg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "EmployeeEvaluations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "EmployeeEvaluations");
        }
    }
}
