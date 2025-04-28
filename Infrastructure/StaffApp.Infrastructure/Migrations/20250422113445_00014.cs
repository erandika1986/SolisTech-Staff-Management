using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00014 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalaryAddon_AspNetUsers_EmployeeId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalaryAddon_EmployeeId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SalaryAddon",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SalaryAddon",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SalaryAddon",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "SalaryAddon",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "SalaryAddon",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeSalaryId",
                table: "EmployeeSalaryAddon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeSalaryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryHistory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryHistory_EmployeeSalary_EmployeeSalaryId",
                        column: x => x.EmployeeSalaryId,
                        principalTable: "EmployeeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryAddonHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeSalaryHistoryId = table.Column<int>(type: "int", nullable: false),
                    EmployeeSalaryId = table.Column<int>(type: "int", nullable: false),
                    SalaryAddonId = table.Column<int>(type: "int", nullable: false),
                    OriginalValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdjustedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryAddonHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryAddonHistory_EmployeeSalaryHistory_EmployeeSalaryHistoryId",
                        column: x => x.EmployeeSalaryHistoryId,
                        principalTable: "EmployeeSalaryHistory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryAddonHistory_EmployeeSalary_EmployeeSalaryId",
                        column: x => x.EmployeeSalaryId,
                        principalTable: "EmployeeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryAddonHistory_SalaryAddon_SalaryAddonId",
                        column: x => x.SalaryAddonId,
                        principalTable: "SalaryAddon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddon_EmployeeSalaryId",
                table: "EmployeeSalaryAddon",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddonHistory_EmployeeSalaryHistoryId",
                table: "EmployeeSalaryAddonHistory",
                column: "EmployeeSalaryHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddonHistory_EmployeeSalaryId",
                table: "EmployeeSalaryAddonHistory",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddonHistory_SalaryAddonId",
                table: "EmployeeSalaryAddonHistory",
                column: "SalaryAddonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryHistory_EmployeeSalaryId",
                table: "EmployeeSalaryHistory",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryHistory_UserId",
                table: "EmployeeSalaryHistory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalaryAddon_EmployeeSalary_EmployeeSalaryId",
                table: "EmployeeSalaryAddon",
                column: "EmployeeSalaryId",
                principalTable: "EmployeeSalary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalaryAddon_EmployeeSalary_EmployeeSalaryId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryAddonHistory");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryHistory");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalaryAddon_EmployeeSalaryId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "EmployeeSalaryId",
                table: "EmployeeSalaryAddon");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "EmployeeSalaryAddon",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddon_EmployeeId",
                table: "EmployeeSalaryAddon",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalaryAddon_AspNetUsers_EmployeeId",
                table: "EmployeeSalaryAddon",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
