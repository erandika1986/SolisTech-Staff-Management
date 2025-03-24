using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CompanyYear",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CompanyYear",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyYear",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "CompanyYear",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CompanyYear",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CompanyFinancialYear",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CompanyFinancialYear",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyFinancialYear",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "CompanyFinancialYear",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CompanyFinancialYear",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CompanyYear");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CompanyYear");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyYear");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "CompanyYear");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CompanyYear");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CompanyFinancialYear");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CompanyFinancialYear");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyFinancialYear");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "CompanyFinancialYear");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CompanyFinancialYear");
        }
    }
}
