using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00033 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_AspNetRoles_RoleId",
                table: "ProjectMember");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMember_RoleId",
                table: "ProjectMember");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "ProjectMember",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "ProjectMember",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMember_RoleId",
                table: "ProjectMember",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_AspNetRoles_RoleId",
                table: "ProjectMember",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
