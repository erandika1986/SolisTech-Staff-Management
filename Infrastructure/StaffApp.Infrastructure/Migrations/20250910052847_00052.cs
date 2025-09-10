using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00052 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QualificationName",
                table: "UserQualificationDocument",
                newName: "FileType");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "UserQualificationDocument",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "UserQualificationDocument");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "UserQualificationDocument",
                newName: "QualificationName");
        }
    }
}
