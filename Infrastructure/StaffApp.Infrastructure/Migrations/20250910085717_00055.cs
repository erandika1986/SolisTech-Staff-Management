using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00055 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConsiderForSocialSecurityScheme",
                table: "EmployeeSalaryAddonHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsiderForSocialSecurityScheme",
                table: "EmployeeSalaryAddon",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsiderForSocialSecurityScheme",
                table: "EmployeeSalaryAddonHistory");

            migrationBuilder.DropColumn(
                name: "ConsiderForSocialSecurityScheme",
                table: "EmployeeSalaryAddon");
        }
    }
}
