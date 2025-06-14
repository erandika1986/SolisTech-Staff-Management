using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _00035 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SavedFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaveFileURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportAttachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseSupportAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    SupportAttachmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseSupportAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseSupportAttachment_Expense_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseSupportAttachment_SupportAttachment_SupportAttachmentId",
                        column: x => x.SupportAttachmentId,
                        principalTable: "SupportAttachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomeSupportAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IncomeId = table.Column<int>(type: "int", nullable: false),
                    SupportAttachmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeSupportAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeSupportAttachment_Income_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Income",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomeSupportAttachment_SupportAttachment_SupportAttachmentId",
                        column: x => x.SupportAttachmentId,
                        principalTable: "SupportAttachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSupportAttachment_ExpenseId",
                table: "ExpenseSupportAttachment",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseSupportAttachment_SupportAttachmentId",
                table: "ExpenseSupportAttachment",
                column: "SupportAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeSupportAttachment_IncomeId",
                table: "IncomeSupportAttachment",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeSupportAttachment_SupportAttachmentId",
                table: "IncomeSupportAttachment",
                column: "SupportAttachmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseSupportAttachment");

            migrationBuilder.DropTable(
                name: "IncomeSupportAttachment");

            migrationBuilder.DropTable(
                name: "SupportAttachment");
        }
    }
}
