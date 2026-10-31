using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(b => b.BrandId);

            builder.Property(b => b.BrandName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b =>b.MainImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(b => b.ImageFolderId)
               .IsRequired()
               .HasMaxLength(500);
            
            builder.HasOne(b => b.BrandGroup)
                .WithMany(bg => bg.Brands)
                .HasForeignKey(b => b.BrandGroupId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        } 
    }
}
