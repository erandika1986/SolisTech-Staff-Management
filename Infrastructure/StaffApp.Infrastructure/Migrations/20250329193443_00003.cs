using System;
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
                name: "CompanyYearId",
                table: "EmployeeLeaveRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "EmployeeLeaveRequest",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeaveDuration",
                table: "EmployeeLeaveRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "EmployeeLeaveRequest",
                type: "time",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveRequest_CompanyYearId",
                table: "EmployeeLeaveRequest",
                column: "CompanyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLeaveRequest_CompanyYear_CompanyYearId",
                table: "EmployeeLeaveRequest",
                column: "CompanyYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLeaveRequest_CompanyYear_CompanyYearId",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLeaveRequest_CompanyYearId",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "CompanyYearId",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "LeaveDuration",
                table: "EmployeeLeaveRequest");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "EmployeeLeaveRequest");
        }
    }
}
