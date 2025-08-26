using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00042 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyYearId",
                table: "Expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Expense",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Expense_CompanyYearId",
                table: "Expense",
                column: "CompanyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_CompanyYear_CompanyYearId",
                table: "Expense",
                column: "CompanyYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_CompanyYear_CompanyYearId",
                table: "Expense");

            migrationBuilder.DropIndex(
                name: "IX_Expense_CompanyYearId",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "CompanyYearId",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Expense");
        }
    }
}
