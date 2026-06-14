using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasConversion(d => d.Value, v => new Description(v))
            .HasMaxLength(100);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasConversion(m => m.Value, v => new Money(v))
            .HasPrecision(18, 2);

        builder.ComplexProperty(t => t.Dates, dates =>
        {
            dates.Property(d => d.CreatedAt);
            dates.Property(d => d.DueDate);
        });

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

        builder.HasOne(t => t.LoanPayable)
            .WithMany()
            .HasForeignKey(t => t.LoanPayableId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(t => t.Investment)
            .WithMany()
            .HasForeignKey(t => t.InvestmentId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(t => t.Goal)
            .WithMany()
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
