using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignReportingManagerId",
                table: "EmployeeLeaveRequest",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveRequest_AssignReportingManagerId",
                table: "EmployeeLeaveRequest",
                column: "AssignReportingManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeaveRequest_AspNetUsers_AssignReportingManagerId",
                table: "EmployeeLeaveRequest",
                column: "AssignReportingManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeaveRequest_AspNetUsers_AssignReportingManagerId",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeaveRequest_AssignReportingManagerId",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "AssignReportingManagerId",
                table: "EmployeeLeaveRequest");
        }
    }
}
