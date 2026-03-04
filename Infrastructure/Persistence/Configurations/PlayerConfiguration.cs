using CodeBattleArena.Domain.Players;
using CodeBattleArena.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class PlayerConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne<ApplicationUser>()
                      .WithOne()
                      .HasForeignKey<Player>(p => p.IdentityId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(p => p.Stats);
            builder.OwnsOne(p => p.Wallet);
            builder.OwnsOne(s => s.Profile, settings =>
            {
                settings.Property(x => x.Name).HasMaxLength(50).IsRequired();
            });

            builder.HasOne(p => p.League)
                .WithMany(t => t.Players)
                .HasForeignKey(s => s.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
