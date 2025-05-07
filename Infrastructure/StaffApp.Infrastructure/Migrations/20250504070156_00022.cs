using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00022 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompnayYearId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMonthlySalary_CompnayYearId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.DropColumn(
                name: "CompnayYearId",
                table: "EmployeeMonthlySalary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompnayYearId",
                table: "EmployeeMonthlySalary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalary_CompnayYearId",
                table: "EmployeeMonthlySalary",
                column: "CompnayYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompnayYearId",
                table: "EmployeeMonthlySalary",
                column: "CompnayYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
