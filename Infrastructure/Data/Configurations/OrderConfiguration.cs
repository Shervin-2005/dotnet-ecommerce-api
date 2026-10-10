using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(20);

        builder.Property(x => x.SubtotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(x => x.RecipientName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Province)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.AddressLine)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Version)
            .IsConcurrencyToken();
        
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}