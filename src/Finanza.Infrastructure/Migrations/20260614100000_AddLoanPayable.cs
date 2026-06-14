using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finanza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanPayable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanPayables",
                columns: table => new
                {
                    Id           = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreditorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TotalAmount  = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    StartDate    = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate      = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes        = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_LoanPayables", x => x.Id));

            migrationBuilder.CreateTable(
                name: "LoanPayableInstallments",
                columns: table => new
                {
                    Id            = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoanPayableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number        = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount        = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate       = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt        = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPayableInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanPayableInstallments_LoanPayables_LoanPayableId",
                        column: x => x.LoanPayableId,
                        principalTable: "LoanPayables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "LoanPayableId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayableInstallments_LoanPayableId",
                table: "LoanPayableInstallments",
                column: "LoanPayableId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanPayableId",
                table: "Transactions",
                column: "LoanPayableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_LoanPayables_LoanPayableId",
                table: "Transactions",
                column: "LoanPayableId",
                principalTable: "LoanPayables",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_LoanPayables_LoanPayableId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LoanPayableId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LoanPayableId",
                table: "Transactions");

            migrationBuilder.DropTable(name: "LoanPayableInstallments");
            migrationBuilder.DropTable(name: "LoanPayables");
        }
    }
}
