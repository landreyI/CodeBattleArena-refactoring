using CodeBattleArena.Domain.TaskLanguages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class TaskLanguageConfiguration : IEntityTypeConfiguration<TaskLanguage>
    {
        public void Configure(EntityTypeBuilder<TaskLanguage> builder)
        {
            builder.HasKey(tk => tk.Id);
            builder.HasIndex(tk => new { tk.ProgrammingTaskId, tk.ProgrammingLangId }).IsUnique();

            builder.Property(t => t.Preparation).IsRequired();
            builder.Property(t => t.VerificationCode).IsRequired();

            builder.HasOne(s => s.ProgrammingLang)
                .WithMany(t => t.TaskLanguages)
                .HasForeignKey(s => s.ProgrammingLangId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ProgrammingTask)
                .WithMany(t => t.TaskLanguages)
                .HasForeignKey(s => s.ProgrammingTaskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
