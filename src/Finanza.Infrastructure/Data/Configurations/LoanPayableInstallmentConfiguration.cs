using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class LoanPayableInstallmentConfiguration : IEntityTypeConfiguration<LoanPayableInstallment>
{
    public void Configure(EntityTypeBuilder<LoanPayableInstallment> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasConversion(m => m.Value, v => new Money(v))
            .HasPrecision(18, 2);

        builder.Ignore(i => i.IsPaid);
        builder.Ignore(i => i.IsOverdue);

        builder.HasOne(i => i.LoanPayable)
            .WithMany(l => l.Installments)
            .HasForeignKey(i => i.LoanPayableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
