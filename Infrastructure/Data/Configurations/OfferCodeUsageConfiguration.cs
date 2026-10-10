using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OfferCodeUsageConfiguration
    : IEntityTypeConfiguration<OfferCodeUsage>
{
    public void Configure(EntityTypeBuilder<OfferCodeUsage> builder)
    {
        builder.HasKey(x => x.OfferCodeUsageId);

        builder.Property(x => x.CodeSnapshot)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.UsedAt)
            .IsRequired();
        
        builder.HasIndex(x => x.OrderId)
            .IsUnique();

        builder.HasOne(x => x.OfferCode)
            .WithMany(x => x.Usages)
            .HasForeignKey(x => x.OfferCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Order)
            .WithOne(x => x.OfferCodeUsage)
            .HasForeignKey<OfferCodeUsage>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}