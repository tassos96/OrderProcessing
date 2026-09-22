using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shipping.Domain.Entities;

namespace Shipping.Infrastructure.Persistence.Configurations;

public sealed class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.OrderId).IsRequired();
        builder.HasIndex(s => s.OrderId);

        builder.OwnsOne(s => s.TrackingNumber, t =>
        {
            t.Property(x => x.Value).HasColumnName("TrackingNumber").HasMaxLength(100);
        });

        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(50);

        builder.OwnsOne(s => s.ShippingAddress, a =>
        {
            a.Property(x => x.Street).HasMaxLength(200);
            a.Property(x => x.City).HasMaxLength(100);
            a.Property(x => x.State).HasMaxLength(100);
            a.Property(x => x.PostalCode).HasMaxLength(20);
            a.Property(x => x.Country).HasMaxLength(100);
        });

        builder.Ignore(s => s.DomainEvents);
    }
}
