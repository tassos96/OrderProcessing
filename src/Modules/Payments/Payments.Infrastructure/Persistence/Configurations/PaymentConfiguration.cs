using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OrderId).IsRequired();
        builder.HasIndex(p => p.OrderId);

        builder.OwnsOne(p => p.Amount, m =>
        {
            m.Property(x => x.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            m.Property(x => x.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.TransactionReference).HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasMaxLength(500);
        builder.Ignore(p => p.DomainEvents);
    }
}
