using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLeaveTypeLogic",
                table: "LeaveType",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "LeaveTypeLogic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    StartMonthOfYear = table.Column<int>(type: "int", nullable: false),
                    StartDateOfMonth = table.Column<int>(type: "int", nullable: false),
                    EntitledLeaveCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypeLogic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveTypeLogic_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypeLogic_LeaveTypeId",
                table: "LeaveTypeLogic",
                column: "LeaveTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveTypeLogic");

            migrationBuilder.DropColumn(
                name: "HasLeaveTypeLogic",
                table: "LeaveType");
        }
    }
}
