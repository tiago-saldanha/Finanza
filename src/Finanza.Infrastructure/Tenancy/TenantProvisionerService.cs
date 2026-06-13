using Finanza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Finanza.Infrastructure.Tenancy;

/// <summary>
/// Cria e inicializa o banco SQLite de um novo usuário no momento do cadastro.
/// Usa EF Core Migrate() que executa a migration InitialCreate e cria todas as tabelas.
/// </summary>
public class TenantProvisionerService(IConfiguration configuration)
{
    public void ProvisionTenant(string userId)
    {
        var baseFolder = configuration["TenantDb:BaseFolder"]!;
        Directory.CreateDirectory(baseFolder);

        var dbPath          = Path.Combine(baseFolder, $"user_{userId}.db");
        var connectionString = $"Data Source={dbPath}";

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlite(connectionString)
            .Options;

        using var context = new TenantDbContext(options);
        context.Database.Migrate();
    }
}
