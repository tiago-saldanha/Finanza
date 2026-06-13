using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class LiabilityConfiguration : IEntityTypeConfiguration<Liability>
{
    public void Configure(EntityTypeBuilder<Liability> builder)
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
    }
}
