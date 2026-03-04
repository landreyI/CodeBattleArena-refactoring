using CodeBattleArena.Domain.Notifications;
using CodeBattleArena.Domain.Players;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Content)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(n => new { n.UserId, n.IsRead });

            builder.Property(n => n.Type)
                .HasConversion<string>();

            builder.HasOne<Player>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
