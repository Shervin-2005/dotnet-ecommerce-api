using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OfferCodeConfiguration : IEntityTypeConfiguration<OfferCode>
{
    public void Configure(EntityTypeBuilder<OfferCode> builder)
    {
        builder.HasKey(x => x.OfferCodeId);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(x => x.MinimumOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaximumDiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.UsedCount)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.StartsAt)
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.ToTable(table =>
        {
            table.HasCheckConstraint(
                "CK_OfferCodes_DiscountPercentage",
                "\"DiscountPercentage\" > 0 AND \"DiscountPercentage\" <= 100");

            table.HasCheckConstraint(
                "CK_OfferCodes_ExpiresAt",
                "\"ExpiresAt\" >= \"StartsAt\"");

            table.HasCheckConstraint(
                "CK_OfferCodes_MaximumDiscountAmount",
                "\"MaximumDiscountAmount\" IS NULL OR \"MaximumDiscountAmount\" > 0");
        });
    }
}