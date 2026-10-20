using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SupportConversationConfiguration : IEntityTypeConfiguration<SupportConversation>
{
    public void Configure(EntityTypeBuilder<SupportConversation> builder)
    {
        builder.HasKey(x => x.SupportConversationId);
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.AssignedAdmin)
            .WithMany()
            .HasForeignKey(x => x.AssignedAdminId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("\"Status\" IN ('Open', 'InProgress')");

        builder.HasIndex(x => new { x.Status, x.CreatedAt });

        builder.HasIndex(x => x.AssignedAdminId);
    }
}