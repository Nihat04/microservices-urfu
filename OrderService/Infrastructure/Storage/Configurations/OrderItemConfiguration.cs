using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).HasColumnName("id");
        builder.Property(oi => oi.ProductId).HasColumnName("product_id").IsRequired();
        builder
            .Property(oi => oi.Quantity)
            .HasColumnName("quantity")
            .HasPrecision(12, 4)
            .IsRequired();
        builder
            .Property(oi => oi.PricePerUnit)
            .HasColumnName("price_per_unit")
            .HasPrecision(12, 4)
            .IsRequired();

        builder.Property<Guid>("OrderId").HasColumnName("order_id").IsRequired();

        builder.Ignore(oi => oi.TotalPrice);
        builder.HasIndex("OrderId").HasDatabaseName("ix_order_items_order_id");
        builder.HasIndex(oi => oi.ProductId).HasDatabaseName("ix_order_items_product_id");
    }
}
