using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(i => i.Id);

        builder.OwnsOne(i => i.Sku, s =>
        {
            s.Property(x => x.Value).HasColumnName("Sku").HasMaxLength(50).IsRequired();
            s.HasIndex(x => x.Value).IsUnique();
        });

        builder.OwnsOne(i => i.AvailableQuantity, q =>
        {
            q.Property(x => x.Value).HasColumnName("AvailableQuantity");
        });

        builder.OwnsOne(i => i.ReservedQuantity, q =>
        {
            q.Property(x => x.Value).HasColumnName("ReservedQuantity");
        });

        builder.Ignore(i => i.DomainEvents);
    }
}
