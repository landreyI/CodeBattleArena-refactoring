using CodeBattleArena.Domain.Friendships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20);

            // Настройка отправителя
            builder.HasOne(f => f.Sender)
                   .WithMany(p => p.SentRequests)
                   .HasForeignKey(f => f.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Настройка получателя
            builder.HasOne(f => f.Receiver)
                   .WithMany(p => p.ReceivedRequests)
                   .HasForeignKey(f => f.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(f => new { f.SenderId, f.ReceiverId }).IsUnique();
        }
    }
}
