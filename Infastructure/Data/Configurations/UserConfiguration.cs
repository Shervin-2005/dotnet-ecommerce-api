using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.UserId);

            builder.Property(u => u.FirstName)
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .HasMaxLength(100);

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(u => u.PhoneNumber)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(500);

            builder.Property(u => u.Role)
                .IsRequired()
                 .HasDefaultValue(UserRole.Customer);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.DeletedAt)
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .IsRequired(false);

            builder.Property(u => u.ProfileUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.ImageFolderId)
               .IsRequired()
               .HasMaxLength(500);
        }
    }
}