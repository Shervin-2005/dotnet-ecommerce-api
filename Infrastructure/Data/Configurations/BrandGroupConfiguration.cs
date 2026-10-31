using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class BrandGroupConfiguration : IEntityTypeConfiguration<BrandGroup>
{
    public void Configure(EntityTypeBuilder<BrandGroup> builder)
    {
        builder.HasKey(bg => bg.BrandGroupId);

        builder.Property(bg => bg.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bg => bg.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(bg => bg.Slug)
            .IsUnique();

        builder.Property(bg => bg.DisplayOrder)
            .IsRequired();
    }
}