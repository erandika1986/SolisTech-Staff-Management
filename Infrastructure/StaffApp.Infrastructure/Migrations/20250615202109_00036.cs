using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00036 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "ExpenseSupportAttachment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ExpenseSupportAttachment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ExpenseSupportAttachment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "ExpenseSupportAttachment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "ExpenseSupportAttachment",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ExpenseSupportAttachment");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ExpenseSupportAttachment");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ExpenseSupportAttachment");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "ExpenseSupportAttachment");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "ExpenseSupportAttachment");
        }
    }
}
