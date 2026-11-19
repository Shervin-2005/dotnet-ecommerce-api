using Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.HasKey(x => x.MenuId);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Menu)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}