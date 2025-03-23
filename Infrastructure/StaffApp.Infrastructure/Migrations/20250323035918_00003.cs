using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndDateOfMonth",
                table: "LeaveTypeLogic",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndMonthOfYear",
                table: "LeaveTypeLogic",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateOfMonth",
                table: "LeaveTypeLogic");

            migrationBuilder.DropColumn(
                name: "EndMonthOfYear",
                table: "LeaveTypeLogic");
        }
    }
}
