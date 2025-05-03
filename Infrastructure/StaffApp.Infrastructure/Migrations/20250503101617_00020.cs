using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00020 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompanyYearId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "EmployeeMonthlySalary",
                newName: "MonthlySalaryId");

            migrationBuilder.RenameColumn(
                name: "CompanyYearId",
                table: "EmployeeMonthlySalary",
                newName: "CompnayYearId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeMonthlySalary_CompanyYearId",
                table: "EmployeeMonthlySalary",
                newName: "IX_EmployeeMonthlySalary_CompnayYearId");

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeSalaryId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryComment_EmployeeSalary_EmployeeSalaryId",
                        column: x => x.EmployeeSalaryId,
                        principalTable: "EmployeeSalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySalary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyYearId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlySalary_CompanyYear_CompanyYearId",
                        column: x => x.CompanyYearId,
                        principalTable: "CompanyYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlySalaryComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlySalaryId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySalaryComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlySalaryComment_MonthlySalary_MonthlySalaryId",
                        column: x => x.MonthlySalaryId,
                        principalTable: "MonthlySalary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthlySalary_MonthlySalaryId",
                table: "EmployeeMonthlySalary",
                column: "MonthlySalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryComment_EmployeeSalaryId",
                table: "EmployeeSalaryComment",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlySalary_CompanyYearId",
                table: "MonthlySalary",
                column: "CompanyYearId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlySalaryComment_MonthlySalaryId",
                table: "MonthlySalaryComment",
                column: "MonthlySalaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompnayYearId",
                table: "EmployeeMonthlySalary",
                column: "CompnayYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMonthlySalary_MonthlySalary_MonthlySalaryId",
                table: "EmployeeMonthlySalary",
                column: "MonthlySalaryId",
                principalTable: "MonthlySalary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompnayYearId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMonthlySalary_MonthlySalary_MonthlySalaryId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryComment");

            migrationBuilder.DropTable(
                name: "MonthlySalaryComment");

            migrationBuilder.DropTable(
                name: "MonthlySalary");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMonthlySalary_MonthlySalaryId",
                table: "EmployeeMonthlySalary");

            migrationBuilder.RenameColumn(
                name: "MonthlySalaryId",
                table: "EmployeeMonthlySalary",
                newName: "Month");

            migrationBuilder.RenameColumn(
                name: "CompnayYearId",
                table: "EmployeeMonthlySalary",
                newName: "CompanyYearId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeMonthlySalary_CompnayYearId",
                table: "EmployeeMonthlySalary",
                newName: "IX_EmployeeMonthlySalary_CompanyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMonthlySalary_CompanyYear_CompanyYearId",
                table: "EmployeeMonthlySalary",
                column: "CompanyYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
