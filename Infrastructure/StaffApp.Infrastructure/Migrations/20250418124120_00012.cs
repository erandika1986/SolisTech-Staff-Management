using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_CreatedByUserId",
                table: "EmployeeSalary");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_UpdatedByUserId",
                table: "EmployeeSalary");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalary_CreatedByUserId",
                table: "EmployeeSalary");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalary_UpdatedByUserId",
                table: "EmployeeSalary");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "EmployeeSalary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<decimal>(
                name: "IncrementAmount",
                table: "EmployeeSalary",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "EmployeeSalary",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "EmployeeBankAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrimaryAccount = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeBankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeBankAccount_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeBankAccount_EmployeeId",
                table: "EmployeeBankAccount",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeBankAccount");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "EmployeeSalary",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "IncrementAmount",
                table: "EmployeeSalary",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "EmployeeSalary",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalary_CreatedByUserId",
                table: "EmployeeSalary",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalary_UpdatedByUserId",
                table: "EmployeeSalary",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_CreatedByUserId",
                table: "EmployeeSalary",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalary_AspNetUsers_UpdatedByUserId",
                table: "EmployeeSalary",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
