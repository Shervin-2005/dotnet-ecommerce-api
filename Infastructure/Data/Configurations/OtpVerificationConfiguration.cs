using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
    {
        public void Configure(EntityTypeBuilder<OtpVerification> builder)
        {
            builder.HasKey(o => o.OtpVerificationId);

            builder.Property(o => o.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.CodeHash)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(o => o.ExpiresAt)
                .IsRequired();

            builder.Property(o => o.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(o => new
            {
                o.PhoneNumber,
                o.CreatedAt
            });
        }
    }
}