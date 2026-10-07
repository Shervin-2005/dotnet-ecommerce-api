using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductVariantConfiguration
    : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(x => x.ProductVariantId);

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder.Property(x => x.OriginalPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.StockQuantity)
            .IsRequired();

        builder.Property(x => x.SoldQuantity)
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AttributeValues)
            .WithMany(x => x.ProductVariants)
            .UsingEntity<Dictionary<string, object>>(
                "ProductVariantAttributeValue",
                right => right
                    .HasOne<ProductAttributeValue>()
                    .WithMany()
                    .HasForeignKey("ProductAttributeValueId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<ProductVariant>()
                    .WithMany()
                    .HasForeignKey("ProductVariantId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey(
                        "ProductVariantId",
                        "ProductAttributeValueId");
                });
    }
}