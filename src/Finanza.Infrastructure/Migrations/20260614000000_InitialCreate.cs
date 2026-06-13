using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Finanza.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id             = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name           = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Type           = table.Column<int>(type: "INTEGER", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Accounts", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id    = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name  = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type  = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Assets", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id          = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name        = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_Categories", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    Id            = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name          = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TargetAmount  = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TargetDate    = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Goals", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Investments",
                columns: table => new
                {
                    Id             = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name           = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type           = table.Column<int>(type: "INTEGER", nullable: false),
                    InvestedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrentValue   = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_Investments", x => x.Id));

            migrationBuilder.CreateTable(
                name: "Liabilities",
                columns: table => new
                {
                    Id        = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name      = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type      = table.Column<int>(type: "INTEGER", nullable: false),
                    Value     = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DueDate   = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes     = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_Liabilities", x => x.Id));

            migrationBuilder.CreateTable(
                name: "LoanReceivables",
                columns: table => new
                {
                    Id           = table.Column<Guid>(type: "TEXT", nullable: false),
                    BorrowerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TotalAmount  = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    StartDate    = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate      = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes        = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_LoanReceivables", x => x.Id));

            migrationBuilder.CreateTable(
                name: "PatrimonySnapshots",
                columns: table => new
                {
                    Id               = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date             = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalAssets      = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalLiabilities = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_PatrimonySnapshots", x => x.Id));

            migrationBuilder.CreateTable(
                name: "AssetValueHistories",
                columns: table => new
                {
                    Id      = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssetId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date    = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value   = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetValueHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetValueHistories_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LiabilityInstallments",
                columns: table => new
                {
                    Id          = table.Column<Guid>(type: "TEXT", nullable: false),
                    LiabilityId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number      = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount      = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate     = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt      = table.Column<DateTime>(type: "TEXT", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "LoanInstallments",
                columns: table => new
                {
                    Id               = table.Column<Guid>(type: "TEXT", nullable: false),
                    LoanReceivableId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number           = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount           = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    DueDate          = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaidAt           = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanInstallments_LoanReceivables_LoanReceivableId",
                        column: x => x.LoanReceivableId,
                        principalTable: "LoanReceivables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id                   = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description          = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Amount               = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Dates_CreatedAt      = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Dates_DueDate        = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PaymentDate          = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status               = table.Column<int>(type: "INTEGER", nullable: false),
                    Type                 = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId            = table.Column<Guid>(type: "TEXT", nullable: true),
                    DestinationAccountId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CategoryId           = table.Column<Guid>(type: "TEXT", nullable: true),
                    AssetId              = table.Column<Guid>(type: "TEXT", nullable: true),
                    LiabilityId          = table.Column<Guid>(type: "TEXT", nullable: true),
                    LoanReceivableId     = table.Column<Guid>(type: "TEXT", nullable: true),
                    InvestmentId         = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_DestinationAccountId",
                        column: x => x.DestinationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Investments_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Liabilities_LiabilityId",
                        column: x => x.LiabilityId,
                        principalTable: "Liabilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_LoanReceivables_LoanReceivableId",
                        column: x => x.LoanReceivableId,
                        principalTable: "LoanReceivables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetValueHistories_AssetId",
                table: "AssetValueHistories",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiabilityInstallments_LiabilityId",
                table: "LiabilityInstallments",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanInstallments_LoanReceivableId",
                table: "LoanInstallments",
                column: "LoanReceivableId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DestinationAccountId",
                table: "Transactions",
                column: "DestinationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_InvestmentId",
                table: "Transactions",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LiabilityId",
                table: "Transactions",
                column: "LiabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanReceivableId",
                table: "Transactions",
                column: "LoanReceivableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AssetValueHistories");
            migrationBuilder.DropTable(name: "LiabilityInstallments");
            migrationBuilder.DropTable(name: "LoanInstallments");
            migrationBuilder.DropTable(name: "Transactions");
            migrationBuilder.DropTable(name: "Accounts");
            migrationBuilder.DropTable(name: "Assets");
            migrationBuilder.DropTable(name: "Categories");
            migrationBuilder.DropTable(name: "Goals");
            migrationBuilder.DropTable(name: "Investments");
            migrationBuilder.DropTable(name: "Liabilities");
            migrationBuilder.DropTable(name: "LoanReceivables");
            migrationBuilder.DropTable(name: "PatrimonySnapshots");
        }
    }
}
