
using CodeBattleArena.Domain.QuestParams;
using CodeBattleArena.Domain.Quests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class QuestParamConfiguration : IEntityTypeConfiguration<QuestParam>
    {
        public void Configure(EntityTypeBuilder<QuestParam> builder)
        {
            builder.HasKey(qp => qp.Id);

            builder.Property(qp => qp.Key)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(qp => qp.Value)
                   .HasMaxLength(255)
                   .IsRequired();

            builder.HasIndex(p => new { p.QuestId, p.Key })
                .IsUnique();

            builder.HasOne<Quest>()
                   .WithMany(q => q.Params)
                   .HasForeignKey(qp => qp.QuestId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
