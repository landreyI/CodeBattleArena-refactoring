
using CodeBattleArena.Domain.QuestRewards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class QuestRewardConfiguration : IEntityTypeConfiguration<QuestReward>
    {
        public void Configure(EntityTypeBuilder<QuestReward> builder)
        {
            builder.HasKey(ps => ps.Id);
            builder.HasIndex(ps => new { ps.QuestId, ps.RewardId }).IsUnique();

            builder.HasOne(ps => ps.Quest)
                   .WithMany(p => p.Rewards)
                   .HasForeignKey(ps => ps.QuestId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tr => tr.Reward)
                .WithMany()
                .HasForeignKey(tr => tr.RewardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
