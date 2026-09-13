using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(r => r.ProductReviewId);
 
            builder.Property(r => r.Comment)
                .HasMaxLength(2000);
            
            builder.HasIndex(r => new { r.ProductId, r.UserId })
                .IsUnique();
            
            builder.ToTable(t => t.HasCheckConstraint("CK_ProductReview_Rating_Range", "\"Rating\" >= 1 AND \"Rating\" <= 5"));
 
            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
 
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}

