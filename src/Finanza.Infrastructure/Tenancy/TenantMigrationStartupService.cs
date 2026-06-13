using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finanza.Infrastructure.Tenancy;

/// <summary>
/// Garante que todos os bancos SQLite de tenants existentes estejam no schema atual ao iniciar a aplicação.
/// Para bancos criados antes das migrations formais, aplica as alterações incrementalmente via SQL
/// e registra a migration consolidada no histórico do EF.
/// </summary>
public class TenantMigrationStartupService(
    IConfiguration configuration,
    ILogger<TenantMigrationStartupService> logger) : IHostedService
{
    private const string InitialCreateMigrationId = "20260614000000_InitialCreate";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var baseFolder = configuration["TenantDb:BaseFolder"];
        if (string.IsNullOrEmpty(baseFolder) || !Directory.Exists(baseFolder))
            return Task.CompletedTask;

        foreach (var dbFile in Directory.GetFiles(baseFolder, "user_*.db"))
        {
            try { MigrateIfNeeded(dbFile); }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha ao migrar banco de tenant: {DbFile}", dbFile);
            }
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static void MigrateIfNeeded(string dbPath)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        using var context = new TenantDbContext(options);
        using var conn = context.Database.GetDbConnection();
        conn.Open();

        // 1. Garante tabela de histórico do EF
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                    "ProductVersion" TEXT NOT NULL
                );
                """;
            cmd.ExecuteNonQuery();
        }

        // 2. Verifica se a migration consolidada já está registrada — se sim, nada a fazer
        bool alreadyMigrated;
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"""
                SELECT COUNT(*) FROM "__EFMigrationsHistory"
                WHERE "MigrationId" = '{InitialCreateMigrationId}';
                """;
            alreadyMigrated = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        if (alreadyMigrated) return;

        // 3. Banco existente pré-migration: aplica todas as alterações incrementais via SQL
        //    (idempotente — usa IF NOT EXISTS / OR IGNORE em tudo)

        // Tabelas base
        CreateTableIfNotExists(conn, "Assets", """
            CREATE TABLE IF NOT EXISTS "Assets" (
                "Id"    TEXT NOT NULL CONSTRAINT "PK_Assets" PRIMARY KEY,
                "Name"  TEXT NOT NULL,
                "Type"  INTEGER NOT NULL,
                "Value" TEXT NOT NULL
            )
            """);

        CreateTableIfNotExists(conn, "Liabilities", """
            CREATE TABLE IF NOT EXISTS "Liabilities" (
                "Id"    TEXT NOT NULL CONSTRAINT "PK_Liabilities" PRIMARY KEY,
                "Name"  TEXT NOT NULL,
                "Type"  INTEGER NOT NULL,
                "Value" TEXT NOT NULL
            )
            """);

        CreateTableIfNotExists(conn, "PatrimonySnapshots", """
            CREATE TABLE IF NOT EXISTS "PatrimonySnapshots" (
                "Id"               TEXT NOT NULL CONSTRAINT "PK_PatrimonySnapshots" PRIMARY KEY,
                "Date"             TEXT NOT NULL,
                "TotalAssets"      TEXT NOT NULL,
                "TotalLiabilities" TEXT NOT NULL
            )
            """);

        CreateTableIfNotExists(conn, "AssetValueHistories", """
            CREATE TABLE IF NOT EXISTS "AssetValueHistories" (
                "Id"      TEXT NOT NULL CONSTRAINT "PK_AssetValueHistories" PRIMARY KEY,
                "AssetId" TEXT NOT NULL,
                "Date"    TEXT NOT NULL,
                "Value"   TEXT NOT NULL,
                CONSTRAINT "FK_AssetValueHistories_Assets_AssetId"
                    FOREIGN KEY ("AssetId") REFERENCES "Assets" ("Id") ON DELETE CASCADE
            )
            """);

        CreateTableIfNotExists(conn, "Investments", """
            CREATE TABLE IF NOT EXISTS "Investments" (
                "Id"             TEXT NOT NULL CONSTRAINT "PK_Investments" PRIMARY KEY,
                "Name"           TEXT NOT NULL,
                "Type"           INTEGER NOT NULL,
                "InvestedAmount" TEXT NOT NULL,
                "CurrentValue"   TEXT NOT NULL
            )
            """);

        CreateTableIfNotExists(conn, "Goals", """
            CREATE TABLE IF NOT EXISTS "Goals" (
                "Id"            TEXT NOT NULL CONSTRAINT "PK_Goals" PRIMARY KEY,
                "Name"          TEXT NOT NULL,
                "TargetAmount"  TEXT NOT NULL,
                "CurrentAmount" TEXT NOT NULL,
                "TargetDate"    TEXT NOT NULL
            )
            """);

        CreateTableIfNotExists(conn, "LoanReceivables", """
            CREATE TABLE IF NOT EXISTS "LoanReceivables" (
                "Id"           TEXT NOT NULL CONSTRAINT "PK_LoanReceivables" PRIMARY KEY,
                "BorrowerName" TEXT NOT NULL,
                "TotalAmount"  TEXT NOT NULL,
                "StartDate"    TEXT NOT NULL,
                "DueDate"      TEXT NOT NULL,
                "Notes"        TEXT NULL
            )
            """);

        CreateTableIfNotExists(conn, "LoanInstallments", """
            CREATE TABLE IF NOT EXISTS "LoanInstallments" (
                "Id"               TEXT NOT NULL CONSTRAINT "PK_LoanInstallments" PRIMARY KEY,
                "LoanReceivableId" TEXT NOT NULL,
                "Number"           INTEGER NOT NULL,
                "Amount"           TEXT NOT NULL,
                "DueDate"          TEXT NOT NULL,
                "PaidAt"           TEXT NULL,
                CONSTRAINT "FK_LoanInstallments_LoanReceivables_LoanReceivableId"
                    FOREIGN KEY ("LoanReceivableId") REFERENCES "LoanReceivables" ("Id") ON DELETE CASCADE
            )
            """);

        CreateTableIfNotExists(conn, "LiabilityInstallments", """
            CREATE TABLE IF NOT EXISTS "LiabilityInstallments" (
                "Id"          TEXT NOT NULL CONSTRAINT "PK_LiabilityInstallments" PRIMARY KEY,
                "LiabilityId" TEXT NOT NULL,
                "Number"      INTEGER NOT NULL,
                "Amount"      TEXT NOT NULL,
                "DueDate"     TEXT NOT NULL,
                "PaidAt"      TEXT NULL,
                CONSTRAINT "FK_LiabilityInstallments_Liabilities_LiabilityId"
                    FOREIGN KEY ("LiabilityId") REFERENCES "Liabilities" ("Id") ON DELETE CASCADE
            )
            """);

        // Colunas adicionadas em fases incrementais
        AddColumnIfNotExists(conn, "Transactions",  "DestinationAccountId", "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions",  "AssetId",              "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions",  "LiabilityId",          "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions",  "LoanReceivableId",     "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions",  "InvestmentId",         "TEXT NULL");
        AddColumnIfNotExists(conn, "Liabilities",   "StartDate",            "TEXT NULL");
        AddColumnIfNotExists(conn, "Liabilities",   "DueDate",              "TEXT NULL");
        AddColumnIfNotExists(conn, "Liabilities",   "Notes",                "TEXT NULL");

        // Migração de dados: converter tipos obsoletos para Expense
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "UPDATE \"Transactions\" SET \"Type\" = 1 WHERE \"Type\" IN (3, 4)";
            cmd.ExecuteNonQuery();
        }

        // 4. Substitui histórico de migrations fragmentado pelo ID consolidado
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "DELETE FROM \"__EFMigrationsHistory\"";
            cmd.ExecuteNonQuery();
        }

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"""
                INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('{InitialCreateMigrationId}', '8.0.23');
                """;
            cmd.ExecuteNonQuery();
        }
    }

    private static void CreateTableIfNotExists(System.Data.Common.DbConnection conn, string tableName, string createSql)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0) return;

        using var createCmd = conn.CreateCommand();
        createCmd.CommandText = createSql;
        createCmd.ExecuteNonQuery();
    }

    private static void AddColumnIfNotExists(System.Data.Common.DbConnection conn, string table, string column, string definition)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"PRAGMA table_info({table})";
        var columns = new List<string>();
        using (var reader = checkCmd.ExecuteReader())
            while (reader.Read())
                columns.Add(reader.GetString(1));

        if (columns.Contains(column)) return;

        using var alterCmd = conn.CreateCommand();
        alterCmd.CommandText = $"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}";
        alterCmd.ExecuteNonQuery();
    }
}
