using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pricing.Domain.Entities;

namespace Pricing.Infrastructure.Persistence.Configurations;

public sealed class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
{
    public void Configure(EntityTypeBuilder<PriceRule> builder)
    {
        builder.ToTable("PriceRules");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Sku).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => r.Sku).IsUnique();

        builder.OwnsOne(r => r.BasePrice, m =>
        {
            m.Property(x => x.Amount).HasColumnName("BasePrice").HasPrecision(18, 2);
            m.Property(x => x.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(r => r.DiscountType).HasConversion<string>().HasMaxLength(50);
        builder.Property(r => r.DiscountValue).HasPrecision(18, 2);
        builder.Ignore(r => r.DomainEvents);
    }
}
