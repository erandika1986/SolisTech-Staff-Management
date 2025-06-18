using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00035 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "IncomeSupportAttachment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "IncomeSupportAttachment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "IncomeSupportAttachment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "IncomeSupportAttachment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "IncomeSupportAttachment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "IncomeSupportAttachment");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "IncomeSupportAttachment");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "IncomeSupportAttachment");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "IncomeSupportAttachment");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "IncomeSupportAttachment");
        }
    }
}
