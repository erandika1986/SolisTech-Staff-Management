using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00047 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppraisalStatus",
                table: "AppraisalPeriod",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyYearId",
                table: "AppraisalPeriod",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AppraisalPeriod_CompanyYearId",
                table: "AppraisalPeriod",
                column: "CompanyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppraisalPeriod_CompanyYear_CompanyYearId",
                table: "AppraisalPeriod",
                column: "CompanyYearId",
                principalTable: "CompanyYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppraisalPeriod_CompanyYear_CompanyYearId",
                table: "AppraisalPeriod");

            migrationBuilder.DropIndex(
                name: "IX_AppraisalPeriod_CompanyYearId",
                table: "AppraisalPeriod");

            migrationBuilder.DropColumn(
                name: "AppraisalStatus",
                table: "AppraisalPeriod");

            migrationBuilder.DropColumn(
                name: "CompanyYearId",
                table: "AppraisalPeriod");
        }
    }
}
