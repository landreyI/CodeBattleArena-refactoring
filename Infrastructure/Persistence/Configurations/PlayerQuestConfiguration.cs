using CodeBattleArena.Domain.PlayerQuests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class PlayerQuestConfiguration : IEntityTypeConfiguration<PlayerQuest>
    {
        public void Configure(EntityTypeBuilder<PlayerQuest> builder)
        {
            builder.HasKey(ps => ps.Id);
            builder.HasIndex(pq => new { pq.PlayerId, pq.QuestId }).IsUnique();

            builder.Property(pq => pq.ProgressValue)
                   .HasMaxLength(100)
                   .IsRequired()
                   .HasDefaultValue("0");

            builder.Property(pq => pq.IsCompleted)
                   .HasDefaultValue(false);

            builder.Property(pq => pq.IsRewardClaimed)
                   .HasDefaultValue(false);


            builder.HasOne(pq => pq.Player)
                   .WithMany(p => p.PlayerQuests)
                   .HasForeignKey(pq => pq.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pq => pq.Quest)
                   .WithMany(q => q.PlayerQuests)
                   .HasForeignKey(pq => pq.QuestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
