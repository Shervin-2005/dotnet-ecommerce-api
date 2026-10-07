using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductAttributeValueConfiguration
    : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(
        EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.HasKey(x => x.ProductAttributeValueId);

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new
            {
                x.ProductAttributeId,
                x.Value
            })
            .IsUnique();

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.ProductAttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}