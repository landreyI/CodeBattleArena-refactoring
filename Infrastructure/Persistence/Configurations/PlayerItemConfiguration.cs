
using CodeBattleArena.Domain.PlayerItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class PlayerItemConfiguration : IEntityTypeConfiguration<PlayerItem>
    {
        public void Configure(EntityTypeBuilder<PlayerItem> builder)
        {
            builder.HasKey(ps => ps.Id);
            builder.HasIndex(ps => new { ps.PlayerId, ps.ItemId }).IsUnique();

            builder.Property(pi => pi.AcquiredAt)
               .IsRequired();

            builder.Property(pi => pi.IsEquipped)
                   .HasDefaultValue(false);

            builder.HasOne(ps => ps.Player)
                   .WithMany(p => p.PlayerItems)
                   .HasForeignKey(ps => ps.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Item)
                   .WithMany(s => s.PlayerItems)
                   .HasForeignKey(ps => ps.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
