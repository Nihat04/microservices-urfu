using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Constants;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Storage.Configurations;

public class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder
            .Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(ProductConstants.MAX_PRODUCT_NAME_LENGTH)
            .IsRequired();

        builder
            .Property(p => p.Price)
            .HasColumnName("price")
            .HasPrecision(12, 2)
            .IsRequired()
            .HasDefaultValue(0.00m);

        builder
            .Property(p => p.StockQuantity)
            .HasColumnName("stock_quantity")
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(p => p.BookedQuantity)
            .HasColumnName("booked_quantity")
            .IsRequired()
            .HasDefaultValue(0);

        builder
            .Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();

        builder
            .Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(p => p.Name).HasDatabaseName("ix_products_name");
        builder.HasIndex(p => p.Price).HasDatabaseName("ix_products_price");
        builder.HasIndex(p => p.CreatedAt).HasDatabaseName("ix_products_created_at").IsDescending();

        builder.ToTable(t => t.HasCheckConstraint("ck_products_price_non_negative", "price >= 0"));
        builder.ToTable(t =>
            t.HasCheckConstraint("ck_products_stock_quantity_non_negative", "stock_quantity >= 0")
        );
        builder.ToTable(t =>
            t.HasCheckConstraint("ck_products_stock_quantity_maximum", "stock_quantity <= 10000")
        );
        builder.ToTable(t =>
            t.HasCheckConstraint("ck_products_booked_quantity_non_negative", "booked_quantity >= 0")
        );
        builder.ToTable(t =>
            t.HasCheckConstraint(
                "ck_products_booked_quantity_valid",
                "booked_quantity <= stock_quantity"
            )
        );
    }
}
