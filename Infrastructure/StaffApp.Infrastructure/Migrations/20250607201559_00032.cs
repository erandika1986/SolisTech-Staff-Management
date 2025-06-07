using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00032 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerComment",
                table: "TimeCard");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TimeCard");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TimeCardEntry",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ManagerComment",
                table: "TimeCardEntry",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TimeCardEntry",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerComment",
                table: "TimeCardEntry");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TimeCardEntry");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TimeCardEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerComment",
                table: "TimeCard",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TimeCard",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
