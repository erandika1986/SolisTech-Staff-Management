using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00051 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "UserQualificationDocument");

            migrationBuilder.AddColumn<int>(
                name: "DocumentNameId",
                table: "UserQualificationDocument",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OtherName",
                table: "UserQualificationDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentName",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmployeeDocumentCategory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentName", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserQualificationDocument_DocumentNameId",
                table: "UserQualificationDocument",
                column: "DocumentNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQualificationDocument_DocumentName_DocumentNameId",
                table: "UserQualificationDocument",
                column: "DocumentNameId",
                principalTable: "DocumentName",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQualificationDocument_DocumentName_DocumentNameId",
                table: "UserQualificationDocument");

            migrationBuilder.DropTable(
                name: "DocumentName");

            migrationBuilder.DropIndex(
                name: "IX_UserQualificationDocument_DocumentNameId",
                table: "UserQualificationDocument");

            migrationBuilder.DropColumn(
                name: "DocumentNameId",
                table: "UserQualificationDocument");

            migrationBuilder.DropColumn(
                name: "OtherName",
                table: "UserQualificationDocument");

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "UserQualificationDocument",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
