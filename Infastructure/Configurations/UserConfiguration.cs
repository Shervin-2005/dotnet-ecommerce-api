using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infastructure.Configurations
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
            .HasMaxLength(20);

            builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();

            builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
