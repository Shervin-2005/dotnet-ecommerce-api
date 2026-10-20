using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.HasKey(x => x.SupportMessageId);
        
        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000);
        
        builder.Property(x => x.SentAt)
            .IsRequired();
        
        builder.Property(x => x.IsRead)
            .IsRequired();
        
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.SupportConversationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => new
        {
            x.SupportConversationId,
            x.SentAt,
            x.SupportMessageId
        });

        builder.HasIndex(x => new
        {
            x.SupportConversationId,
            x.IsRead
        });
    }
}