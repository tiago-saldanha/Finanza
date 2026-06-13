using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class PatrimonySnapshotConfiguration : IEntityTypeConfiguration<PatrimonySnapshot>
{
    public void Configure(EntityTypeBuilder<PatrimonySnapshot> builder)
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

        builder.Ignore(s => s.NetWorth);
    }
}
