using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finanza.Infrastructure.Migrations
{
    public partial class AddGoalIdToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GoalId",
                table: "Transactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GoalId",
                table: "Transactions",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Goals_GoalId",
                table: "Transactions",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Goals_GoalId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_GoalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "Transactions");
        }
    }
}
