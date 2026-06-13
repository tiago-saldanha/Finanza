using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Data;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category>    Categories   { get; set; }
    public DbSet<Account>     Accounts     { get; set; }
    public DbSet<Asset>              Assets             { get; set; }
    public DbSet<Liability>          Liabilities        { get; set; }
    public DbSet<PatrimonySnapshot>  PatrimonySnapshots { get; set; }
    public DbSet<AssetValueHistory>  AssetValueHistories { get; set; }
    public DbSet<Investment>         Investments         { get; set; }
    public DbSet<Goal>               Goals               { get; set; }
    public DbSet<LoanReceivable>        LoanReceivables        { get; set; }
    public DbSet<LoanInstallment>       LoanInstallments       { get; set; }
    public DbSet<LiabilityInstallment>  LiabilityInstallments  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(60);

            builder.Property(c => c.Description)
                .HasMaxLength(100);

            builder.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Transaction>(builder =>
        {
            builder.Property(t => t.Description)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.ComplexProperty(t => t.Dates, dates => dates.Property(d => d.CreatedAt));
            builder.ComplexProperty(t => t.Dates, dates => dates.Property(d => d.DueDate));

            builder.Property(t => t.Amount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(t => t.DestinationAccount)
                .WithMany(a => a.IncomingTransfers)
                .HasForeignKey(t => t.DestinationAccountId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(t => t.Asset)
                .WithMany()
                .HasForeignKey(t => t.AssetId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(t => t.Liability)
                .WithMany()
                .HasForeignKey(t => t.LiabilityId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(t => t.LoanReceivable)
                .WithMany()
                .HasForeignKey(t => t.LoanReceivableId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        modelBuilder.Entity<Account>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(60);

            builder.Property(a => a.InitialBalance)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Asset>(builder =>
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.Property(a => a.Value)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Liability>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.Property(l => l.Value)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(l => l.Notes).HasMaxLength(300);

            builder.Ignore(l => l.TotalPaid);
            builder.Ignore(l => l.Balance);
            builder.Ignore(l => l.IsSettled);
            builder.Ignore(l => l.HasOverdue);
        });

        modelBuilder.Entity<LiabilityInstallment>(builder =>
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Amount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.HasOne(i => i.Liability)
                .WithMany(l => l.Installments)
                .HasForeignKey(i => i.LiabilityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Goal>(builder =>
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.Property(g => g.TargetAmount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(g => g.CurrentAmount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Ignore(g => g.ProgressRate);
            builder.Ignore(g => g.Remaining);
            builder.Ignore(g => g.IsCompleted);
        });

        modelBuilder.Entity<Investment>(builder =>
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.Property(i => i.InvestedAmount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(i => i.CurrentValue)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Ignore(i => i.Return);
            builder.Ignore(i => i.ReturnRate);
        });

        modelBuilder.Entity<AssetValueHistory>(builder =>
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Value)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.HasOne(h => h.Asset)
                .WithMany(a => a.ValueHistory)
                .HasForeignKey(h => h.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LoanReceivable>(builder =>
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.BorrowerName)
                .IsRequired()
                .HasConversion(d => d.Value, v => new Description(v))
                .HasMaxLength(100);

            builder.Property(l => l.TotalAmount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(l => l.Notes).HasMaxLength(300);
        });

        modelBuilder.Entity<LoanInstallment>(builder =>
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Amount)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.HasOne(i => i.LoanReceivable)
                .WithMany(l => l.Installments)
                .HasForeignKey(i => i.LoanReceivableId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PatrimonySnapshot>(builder =>
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.TotalAssets)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);

            builder.Property(s => s.TotalLiabilities)
                .IsRequired()
                .HasConversion(m => m.Value, v => new Money(v))
                .HasPrecision(18, 2);
        });
    }
}
