using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HalfDaySessionType",
                table: "EmployeeLeaveRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShortLeaveSessionType",
                table: "EmployeeLeaveRequest",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HalfDaySessionType",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "ShortLeaveSessionType",
                table: "EmployeeLeaveRequest");
        }
    }
}
