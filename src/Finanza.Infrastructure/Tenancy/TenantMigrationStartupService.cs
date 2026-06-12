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
