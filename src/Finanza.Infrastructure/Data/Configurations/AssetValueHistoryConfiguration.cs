using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class AssetValueHistoryConfiguration : IEntityTypeConfiguration<AssetValueHistory>
{
    public void Configure(EntityTypeBuilder<AssetValueHistory> builder)
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
    }
}
