using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMPLOYEE.Migrations
{
    /// <inheritdoc />
    public partial class Period : Migration
    {
        /// <inheritdoc />
        ///    migrationBuilder.Sql(

        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            // 2️⃣ تحويل القيم القديمة (yyyy-MM) إلى DateTime
            migrationBuilder.Sql(
                @"UPDATE EmployeeEvaluations
                  SET Period = TRY_CONVERT(DATETIME, Period + '-01')"
            );

            // 3️⃣ ملء أي قيم NULL بقيمة افتراضية (أول يوم من الشهر الحالي)
            migrationBuilder.Sql(
                @"UPDATE EmployeeEvaluations
                  SET Period = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
                  WHERE Period IS NULL"
            );

            // 4️⃣ إعادة العمود ليصبح NOT NULL بعد تنظيف البيانات
            migrationBuilder.AlterColumn<DateTime>(
                name: "Period",
                table: "EmployeeEvaluations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // الرجوع للنوع السابق (nvarchar(max))
            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "EmployeeEvaluations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
