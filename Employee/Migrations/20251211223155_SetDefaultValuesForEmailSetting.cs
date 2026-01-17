using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPLOYEE.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultValuesForEmailSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSetting");

            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: ""),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "smtp.gmail.com"),
                    Port = table.Column<int>(type: "int", nullable: false, defaultValue: 465),
                    EnableSSL = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSetting");

            migrationBuilder.CreateTable(
                name: "EmailSettins",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnableSSL = table.Column<bool>(type: "bit", nullable: false),
                    Host = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettind", x => x.id);
                });
        }
    }
}
