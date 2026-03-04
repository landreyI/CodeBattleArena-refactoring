using CodeBattleArena.Domain.ProgrammingTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class ProgrammingTaskConfiguration : IEntityTypeConfiguration<ProgrammingTask>
    {
        public void Configure(EntityTypeBuilder<ProgrammingTask> builder)
        {
            builder.HasKey(pl => pl.Id);
            builder.Property(pl => pl.Name).IsRequired().HasMaxLength(100);
            builder.Property(pl => pl.Description).IsRequired();
            builder.Property(pl => pl.Difficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(t => t.CreatedAt).IsRequired();

            builder.HasIndex(t => t.Name);

            builder.HasOne(s => s.Author)
                .WithMany(t => t.Tasks)
                .HasForeignKey(s => s.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
