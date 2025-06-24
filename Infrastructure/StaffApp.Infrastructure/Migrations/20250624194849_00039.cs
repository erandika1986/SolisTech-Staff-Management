using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00039 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "InvoiceDetail",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyYearId",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetail_EmployeeId",
                table: "InvoiceDetail",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CompanyYearId",
                table: "Invoice",
                column: "CompanyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_CompanyYear_CompanyYearId",
                table: "Invoice",
                column: "CompanyYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceDetail_AspNetUsers_EmployeeId",
                table: "InvoiceDetail",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_CompanyYear_CompanyYearId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceDetail_AspNetUsers_EmployeeId",
                table: "InvoiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetail_EmployeeId",
                table: "InvoiceDetail");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_CompanyYearId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "InvoiceDetail");

            migrationBuilder.DropColumn(
                name: "CompanyYearId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Invoice");
        }
    }
}
