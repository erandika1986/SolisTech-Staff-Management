using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00019 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedValue",
                table: "EmployeeMonthlySalaryAddon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPayeApplicable",
                table: "EmployeeMonthlySalaryAddon",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalValue",
                table: "EmployeeMonthlySalaryAddon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EmployeeMonthlySalary",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustedValue",
                table: "EmployeeMonthlySalaryAddon");

            migrationBuilder.DropColumn(
                name: "IsPayeApplicable",
                table: "EmployeeMonthlySalaryAddon");

            migrationBuilder.DropColumn(
                name: "OriginalValue",
                table: "EmployeeMonthlySalaryAddon");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EmployeeMonthlySalary");
        }
    }
}
