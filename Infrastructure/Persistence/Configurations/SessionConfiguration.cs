
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(g => g.Name).HasMaxLength(20).IsRequired();

            builder.OwnsOne(s => s.Access, settings =>
            {
                settings.Property(x => x.Password).HasMaxLength(100);
                settings.Property(x => x.MaxPeople).IsRequired();
                settings.Property(g => g.Type)
                        .HasMaxLength(20)
                        .HasConversion<string>();
            });

            builder.Property(g => g.Status)
                   .HasMaxLength(20)
                   .HasConversion<string>();

            builder.HasIndex(s => s.CreatorId);

            builder.HasOne(s => s.ProgrammingTask)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.ProgrammingTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ProgrammingLang)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.ProgrammingLangId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Player>()
                .WithMany()
                .HasForeignKey(s => s.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
