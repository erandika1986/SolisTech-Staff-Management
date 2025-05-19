using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00028 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Project_ProjectId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_ProjectId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Project",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectId",
                table: "Project",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Project_ProjectId",
                table: "Project",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }
    }
}
