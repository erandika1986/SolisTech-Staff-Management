using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00029 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProjectDocument",
                newName: "SavedFileName");

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "ProjectDocument",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Project",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "ProjectDocument");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "SavedFileName",
                table: "ProjectDocument",
                newName: "Name");
        }
    }
}
