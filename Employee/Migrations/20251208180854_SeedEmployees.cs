using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMPLOYEE.Migrations
{
    /// <inheritdoc />
    public partial class SeedEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "FullName", "ManagerId", "Position", "Salary" },
                values: new object[,]
                {
                    { 1, "khaled.hassan1@gmail.com", "Khaled Hassan", "98f51f06-3fcd-4e23-a84a-97b431f905b0", "Developer", 6500m },
                    { 2, "moh.sami2@gmail.com", "Mohammed Sami", "98f51f06-3fcd-4e23-a84a-97b431f905b0", "HR Assistant", 5000m },
                    { 3, "rami.ali3@gmail.com", "Rami Ali", "98f51f06-3fcd-4e23-a84a-97b431f905b0", "Accountant", 5800m },
                    { 4, "fares.ziad4@gmail.com", "Fares Ziad", "98f51f06-3fcd-4e23-a84a-97b431f905b0", "Sales Rep", 5400m },
                    { 5, "hazem.saleh5@gmail.com", "Hazem Saleh", "98f51f06-3fcd-4e23-a84a-97b431f905b0", "IT Support", 5200m },
                    { 6, "saleh.omar1@gmail.com", "Saleh Omar", "c448faf3-53d5-411f-91c6-2a28ef0fbd94", "Developer", 6700m },
                    { 7, "yasser.sami2@gmail.com", "Yasser Sami", "c448faf3-53d5-411f-91c6-2a28ef0fbd94", "Project Coord", 5300m },
                    { 8, "mazin.adel3@gmail.com", "Mazin Adel", "c448faf3-53d5-411f-91c6-2a28ef0fbd94", "Marketing", 5100m },
                    { 9, "anas.fahad4@gmail.com", "Anas Fahad", "c448faf3-53d5-411f-91c6-2a28ef0fbd94", "Data Entry", 4800m },
                    { 10, "rashed.amer5@gmail.com", "Rashed Amer", "c448faf3-53d5-411f-91c6-2a28ef0fbd94", "Customer Care", 4900m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
