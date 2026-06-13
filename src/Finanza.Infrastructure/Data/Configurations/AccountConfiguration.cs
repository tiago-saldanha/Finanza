using Finanza.Domain.Entities;
using Finanza.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanza.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
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
    }
}
