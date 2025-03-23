using System;
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
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "Department",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 640, DateTimeKind.Local).AddTicks(4137));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Department",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 638, DateTimeKind.Local).AddTicks(3030));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDate",
                table: "Department",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 640, DateTimeKind.Local).AddTicks(4137),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Department",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2025, 3, 23, 10, 18, 56, 638, DateTimeKind.Local).AddTicks(3030),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
