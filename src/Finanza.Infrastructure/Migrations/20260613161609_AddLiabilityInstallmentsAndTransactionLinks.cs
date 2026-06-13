using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finanza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLiabilityInstallmentsAndTransactionLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LiabilityId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LoanReceivableId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Liabilities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Liabilities",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Liabilities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LiabilityInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiabilityInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiabilityInstallments_Liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "Liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LiabilityId",
                table: "Transactions",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanReceivableId",
                table: "Transactions",
                column: "LoanReceivableId");

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityInstallments_LiabilityId",
                table: "LiabilityInstallments",
                column: "LiabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Liabilities_LiabilityId",
                table: "Transactions",
                column: "LiabilityId",
                principalTable: "Liabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_LoanReceivables_LoanReceivableId",
                table: "Transactions",
                column: "LoanReceivableId",
                principalTable: "LoanReceivables",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Liabilities_LiabilityId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_LoanReceivables_LoanReceivableId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "LiabilityInstallments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LiabilityId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LoanReceivableId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LiabilityId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LoanReceivableId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Liabilities");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Liabilities");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Liabilities");
        }
    }
}
