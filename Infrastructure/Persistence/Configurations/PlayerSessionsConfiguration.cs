
using CodeBattleArena.Domain.PlayerSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class PlayerSessionsConfiguration : IEntityTypeConfiguration<PlayerSession>
    {
        public void Configure(EntityTypeBuilder<PlayerSession> builder)
        {
            builder.HasKey(s => s.Id);
            builder.HasIndex(ps => new { ps.PlayerId, ps.SessionId }).IsUnique();

            builder.OwnsOne(ps => ps.Result, res =>
            {
                res.Property(r => r.Time).HasMaxLength(50);
            });

            builder.HasOne(ps => ps.Player)
                   .WithMany(p => p.PlayerSessions)
                   .HasForeignKey(ps => ps.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Session)
                   .WithMany(s => s.PlayerSessions)
                   .HasForeignKey(ps => ps.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
