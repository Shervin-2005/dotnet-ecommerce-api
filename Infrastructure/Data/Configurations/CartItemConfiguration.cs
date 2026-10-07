using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.CartItemId);

            builder.HasIndex(ci => new
                {
                    ci.CartId,
                    ci.ProductId
                })
                .IsUnique()
                .HasFilter("\"ProductVariantId\" IS NULL");

            builder.HasIndex(ci => new
                {
                    ci.CartId,
                    ci.ProductId,
                    ci.ProductVariantId
                })
                .IsUnique()
                .HasFilter("\"ProductVariantId\" IS NOT NULL");

            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
                "CK_CartItem_Quantity_Positive",
                "\"Quantity\" > 0"));

            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ci => ci.ProductVariant)
                .WithMany()
                .HasForeignKey(ci => ci.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

