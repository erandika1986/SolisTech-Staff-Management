using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00060 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Vehicle",
                newName: "ManufactureName");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Vehicle",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Vehicle",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Vehicle",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Vehicle",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Vehicle",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "ManufactureName",
                table: "Vehicle",
                newName: "Model");
        }
    }
}
