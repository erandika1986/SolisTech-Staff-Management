using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00013 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncrementAmount",
                table: "EmployeeSalary");

            migrationBuilder.DropColumn(
                name: "IncrementPercentage",
                table: "EmployeeSalary");

            migrationBuilder.DropColumn(
                name: "IncrementReason",
                table: "EmployeeSalary");

            migrationBuilder.DropColumn(
                name: "IsIncrement",
                table: "EmployeeSalary");

            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "EmployeeSalary",
                newName: "BaseSalary");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                table: "EmployeeSalary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "EmployeeMonthlySalary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeSalaryId = table.Column<int>(type: "int", nullable: false),
                    CompanyYearId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    GrossSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalForPF = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeduction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeMonthlySalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeMonthlySalary_CompanyYear_CompanyYearId",
                        column: x => x.CompanyYearId,
                        principalTable: "CompanyYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeMonthlySalary_EmployeeSalary_EmployeeSalaryId",
                        column: x => x.EmployeeSalaryId,
                        principalTable: "EmployeeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalaryAddon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AddonType = table.Column<int>(type: "int", nullable: false),
                    ProportionType = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplyForAllEmployees = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryAddon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeMonthlySalaryAddon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeMonthlySalaryId = table.Column<int>(type: "int", nullable: false),
                    SalaryAddonId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeMonthlySalaryAddon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeMonthlySalaryAddon_EmployeeMonthlySalary_EmployeeMonthlySalaryId",
                        column: x => x.EmployeeMonthlySalaryId,
                        principalTable: "EmployeeMonthlySalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeMonthlySalaryAddon_SalaryAddon_SalaryAddonId",
                        column: x => x.SalaryAddonId,
                        principalTable: "SalaryAddon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryAddon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeSalaryAddon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryAddon_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryAddon_SalaryAddon_SalaryAddonId",
                        column: x => x.SalaryAddonId,
                        principalTable: "SalaryAddon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalary_CompanyYearId",
                table: "EmployeeMonthlySalary",
                column: "CompanyYearId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalary_EmployeeSalaryId",
                table: "EmployeeMonthlySalary",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalaryAddon_EmployeeMonthlySalaryId",
                table: "EmployeeMonthlySalaryAddon",
                column: "EmployeeMonthlySalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalaryAddon_SalaryAddonId",
                table: "EmployeeMonthlySalaryAddon",
                column: "SalaryAddonId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddon_EmployeeId",
                table: "EmployeeSalaryAddon",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryAddon_SalaryAddonId",
                table: "EmployeeSalaryAddon",
                column: "SalaryAddonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeMonthlySalaryAddon");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryAddon");

            migrationBuilder.DropTable(
                name: "EmployeeMonthlySalary");

            migrationBuilder.DropTable(
                name: "SalaryAddon");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                table: "EmployeeSalary");

            migrationBuilder.RenameColumn(
                name: "BaseSalary",
                table: "EmployeeSalary",
                newName: "Salary");

            migrationBuilder.AddColumn<decimal>(
                name: "IncrementAmount",
                table: "EmployeeSalary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IncrementPercentage",
                table: "EmployeeSalary",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncrementReason",
                table: "EmployeeSalary",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncrement",
                table: "EmployeeSalary",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
