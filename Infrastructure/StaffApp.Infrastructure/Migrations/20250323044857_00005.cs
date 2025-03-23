using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Department",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Department",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 638, DateTimeKind.Local).AddTicks(3030));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Department",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Department",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 640, DateTimeKind.Local).AddTicks(4137));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Department",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Department");
        }
    }
}
