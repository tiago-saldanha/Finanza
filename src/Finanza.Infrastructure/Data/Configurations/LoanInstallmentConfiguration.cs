using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class LoanInstallmentConfiguration : IEntityTypeConfiguration<LoanInstallment>
{
    public void Configure(EntityTypeBuilder<LoanInstallment> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasConversion(m => m.Value, v => new Money(v))
            .HasPrecision(18, 2);

        builder.Ignore(i => i.IsPaid);
        builder.Ignore(i => i.IsOverdue);

        builder.HasOne(i => i.LoanReceivable)
            .WithMany(l => l.Installments)
            .HasForeignKey(i => i.LoanReceivableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
