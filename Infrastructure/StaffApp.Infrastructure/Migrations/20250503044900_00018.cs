using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalForPF",
                table: "EmployeeMonthlySalary",
                newName: "TotalEarning");

            migrationBuilder.RenameColumn(
                name: "GrossSalary",
                table: "EmployeeMonthlySalary",
                newName: "EmployerContribution");

            migrationBuilder.AddColumn<decimal>(
                name: "BasicSalary",
                table: "EmployeeMonthlySalary",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicSalary",
                table: "EmployeeMonthlySalary");

            migrationBuilder.RenameColumn(
                name: "TotalEarning",
                table: "EmployeeMonthlySalary",
                newName: "TotalForPF");

            migrationBuilder.RenameColumn(
                name: "EmployerContribution",
                table: "EmployeeMonthlySalary",
                newName: "GrossSalary");
        }
    }
}
