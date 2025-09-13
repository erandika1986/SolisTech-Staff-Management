using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00058 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehiclePurpose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclePurpose", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleAssignedPurpose",
                columns: table => new
                {
                    VehiclePurposeId = table.Column<int>(type: "int", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAssignedPurpose", x => new { x.VehicleId, x.VehiclePurposeId });
                    table.ForeignKey(
                        name: "FK_VehicleAssignedPurpose_VehiclePurpose_VehiclePurposeId",
                        column: x => x.VehiclePurposeId,
                        principalTable: "VehiclePurpose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleAssignedPurpose_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignedPurpose_VehiclePurposeId",
                table: "VehicleAssignedPurpose",
                column: "VehiclePurposeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleAssignedPurpose");

            migrationBuilder.DropTable(
                name: "VehiclePurpose");
        }
    }
}
