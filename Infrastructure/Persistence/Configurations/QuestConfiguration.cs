
using CodeBattleArena.Domain.Quests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class QuestConfiguration : IEntityTypeConfiguration<Quest>
    {
        public void Configure(EntityTypeBuilder<Quest> builder)
        {
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Name).IsRequired().HasMaxLength(100);
            builder.Property(q => q.Type).HasConversion<string>().HasMaxLength(40);


            builder.OwnsOne(q => q.Recurrence, r =>
            {
                r.Property(p => p.IsRepeatable).HasDefaultValue(false);
                r.Property(p => p.RepeatAfterDays).HasColumnName("RepeatDays");
            });
        }
    }
}
