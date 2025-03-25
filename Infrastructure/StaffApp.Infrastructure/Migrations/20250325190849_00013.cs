using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveTypeConfig_AspNetUsers_CreatedByUserId",
                table: "LeaveTypeConfig");

            migrationBuilder.DropForeignKey(
                name: "FK_LeaveTypeConfig_AspNetUsers_UpdatedByUserId",
                table: "LeaveTypeConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypeConfig_CreatedByUserId",
                table: "LeaveTypeConfig");

            migrationBuilder.DropIndex(
                name: "IX_LeaveTypeConfig_UpdatedByUserId",
                table: "LeaveTypeConfig");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "LeaveTypeConfig");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "LeaveTypeConfig");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LeaveTypeConfig");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "LeaveTypeConfig");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "LeaveTypeConfig");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "LeaveTypeConfig",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "LeaveTypeConfig",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LeaveTypeConfig",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "LeaveTypeConfig",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "LeaveTypeConfig",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypeConfig_CreatedByUserId",
                table: "LeaveTypeConfig",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypeConfig_UpdatedByUserId",
                table: "LeaveTypeConfig",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveTypeConfig_AspNetUsers_CreatedByUserId",
                table: "LeaveTypeConfig",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveTypeConfig_AspNetUsers_UpdatedByUserId",
                table: "LeaveTypeConfig",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
