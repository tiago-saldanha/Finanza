using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Finanza.Infrastructure.Tenancy;

/// <summary>
/// Cria e inicializa o banco SQLite de um novo usuário no momento do cadastro.
/// Para bancos existentes (antes das migrations), aplica esquema incremental
/// sem recriar as tabelas já existentes.
/// </summary>
public class TenantProvisionerService(IConfiguration configuration)
{
    private const string InitialMigrationId          = "20260612000339_AddTransferTransaction";
    private const string LoanMigrationId             = "20260612003013_AddLoanReceivable";
    private const string PatrimonialLinksMigrationId = "20260613161609_AddLiabilityInstallmentsAndTransactionLinks";
    private const string InvestmentLinkMigrationId   = "20260613200000_AddInvestmentIdToTransaction";

    public void ProvisionTenant(string userId)
    {
        var baseFolder = configuration["TenantDb:BaseFolder"]!;
        Directory.CreateDirectory(baseFolder);

        var dbPath = Path.Combine(baseFolder, $"user_{userId}.db");
        var connectionString = $"Data Source={dbPath}";

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new TenantDbContext(options);

        if (File.Exists(dbPath))
            MigrateExistingDatabase(context);
        else
            context.Database.Migrate(); // banco novo: EF cria todas as tabelas e colunas
    }

    private static void MigrateExistingDatabase(TenantDbContext context)
    {
        using var conn = context.Database.GetDbConnection();
        conn.Open();

        // 1. Garante que a tabela de histórico existe
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

        // 2. Marca migrations como aplicadas para bancos pré-EF
        foreach (var migId in new[] { InitialMigrationId, LoanMigrationId, PatrimonialLinksMigrationId, InvestmentLinkMigrationId })
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"""
                INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
                VALUES ('{migId}', '8.0.0');
                """;
            cmd.ExecuteNonQuery();
        }

        // 3. Adiciona colunas novas se não existirem (incremento sem recriar tabelas)
        AddColumnIfNotExists(conn, "Transactions", "DestinationAccountId", "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions", "AssetId",              "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions", "LiabilityId",          "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions", "LoanReceivableId",     "TEXT NULL");
        AddColumnIfNotExists(conn, "Transactions", "InvestmentId",         "TEXT NULL");
    }

    private static void AddColumnIfNotExists(System.Data.Common.DbConnection conn, string table, string column, string definition)
    {
        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = $"PRAGMA table_info({table})";
        var columns = new List<string>();
        using (var reader = checkCmd.ExecuteReader())
        {
            while (reader.Read())
                columns.Add(reader.GetString(1)); // column name
        }

        if (!columns.Contains(column))
        {
            using var alterCmd = conn.CreateCommand();
            alterCmd.CommandText = $"ALTER TABLE \"{table}\" ADD COLUMN \"{column}\" {definition}";
            alterCmd.ExecuteNonQuery();
        }
    }
}
