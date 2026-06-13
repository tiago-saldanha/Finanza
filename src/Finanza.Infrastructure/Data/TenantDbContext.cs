using Finanza.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Transaction>           Transactions          { get; set; }
    public DbSet<Category>              Categories            { get; set; }
    public DbSet<Account>               Accounts              { get; set; }
    public DbSet<Asset>                 Assets                { get; set; }
    public DbSet<AssetValueHistory>     AssetValueHistories   { get; set; }
    public DbSet<Liability>             Liabilities           { get; set; }
    public DbSet<LiabilityInstallment>  LiabilityInstallments { get; set; }
    public DbSet<Investment>            Investments           { get; set; }
    public DbSet<LoanReceivable>        LoanReceivables       { get; set; }
    public DbSet<LoanInstallment>       LoanInstallments      { get; set; }
    public DbSet<Goal>                  Goals                 { get; set; }
    public DbSet<PatrimonySnapshot>     PatrimonySnapshots    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
    }
}
