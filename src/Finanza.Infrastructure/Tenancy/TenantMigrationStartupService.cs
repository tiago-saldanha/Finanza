using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finanza.Infrastructure.Tenancy;

/// <summary>
/// Aplica migrations incrementais em todos os bancos SQLite de tenants existentes
/// quando a aplicação inicia. Garante que novos campos (ex: DestinationAccountId)
/// sejam adicionados sem recriar os dados existentes.
/// </summary>
public class TenantMigrationStartupService(
    IConfiguration configuration,
    ILogger<TenantMigrationStartupService> logger) : IHostedService
{
    private const string InitialMigrationId = "20260612000339_AddTransferTransaction";
    private const string LoanMigrationId    = "20260612003013_AddLoanReceivable";

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var baseFolder = configuration["TenantDb:BaseFolder"];
        if (string.IsNullOrEmpty(baseFolder) || !Directory.Exists(baseFolder))
            return Task.CompletedTask;

        foreach (var dbFile in Directory.GetFiles(baseFolder, "user_*.db"))
        {
            try
            {
                MigrateIfNeeded(dbFile);
            }
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

        // Garante tabela de histórico
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

        // Marca migration inicial se não estiver registrada
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = $"""
                INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('{InitialMigrationId}', '8.0.0');
                """;
            cmd.ExecuteNonQuery();
        }

        // Adiciona DestinationAccountId se não existir
        AddColumnIfNotExists(conn, "Transactions", "DestinationAccountId", "TEXT NULL");

        // Migration: AddLoanReceivable
        MarkMigrationAppliedIfTablesExist(conn, LoanMigrationId, "LoanReceivables");
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
    }

    private static void MarkMigrationAppliedIfTablesExist(System.Data.Common.DbConnection conn, string migrationId, string tableToCheck)
    {
        // Se a tabela já existe no banco (criada fora de migrations), marca como aplicada
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableToCheck}'";
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

        using var insertCmd = conn.CreateCommand();
        insertCmd.CommandText = $"""
            INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('{migrationId}', '8.0.0');
            """;
        insertCmd.ExecuteNonQuery();

        _ = exists; // tabela pode ou não existir — a migration é marcada de qualquer forma para o Migrate() não recriar
    }

    private static void CreateTableIfNotExists(System.Data.Common.DbConnection conn, string tableName, string createSql)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{tableName}'";
        var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
        if (!exists)
        {
            using var createCmd = conn.CreateCommand();
            createCmd.CommandText = createSql;
            createCmd.ExecuteNonQuery();
        }
    }

    private static void AddColumnIfNotExists(System.Data.Common.DbConnection conn, string table, string column, string definition)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"PRAGMA table_info({table})";
        var columns = new List<string>();
        using (var reader = checkCmd.ExecuteReader())
            while (reader.Read())
                columns.Add(reader.GetString(1));

        if (!columns.Contains(column))
        {
            using var alterCmd = conn.CreateCommand();
            alterCmd.CommandText = $"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}";
            alterCmd.ExecuteNonQuery();
        }
    }
}
