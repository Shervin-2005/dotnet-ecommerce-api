using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class FlashSaleConfiguration : IEntityTypeConfiguration<FlashSale>
{
    public void Configure(EntityTypeBuilder<FlashSale> builder)
    {
        builder.HasKey(x => x.FlashSaleId);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.StartsAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(x => x.EndsAt)
            .HasColumnType("timestamp with time zone");

        builder.HasMany(x => x.Items)
            .WithOne(x => x.FlashSale)
            .HasForeignKey(x => x.FlashSaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}