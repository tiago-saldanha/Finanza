using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finanza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvestmentIdToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvestmentId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InvestmentId",
                table: "Transactions",
                column: "InvestmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Investments_InvestmentId",
                table: "Transactions",
                column: "InvestmentId",
                principalTable: "Investments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Investments_InvestmentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_InvestmentId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InvestmentId",
                table: "Transactions");
        }
    }
}
