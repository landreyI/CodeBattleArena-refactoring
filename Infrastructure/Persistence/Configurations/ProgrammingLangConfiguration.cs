
using CodeBattleArena.Domain.ProgrammingLanguages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class ProgrammingLangConfiguration : IEntityTypeConfiguration<ProgrammingLang>
    {
        public void Configure(EntityTypeBuilder<ProgrammingLang> builder)
        {
            builder.HasKey(pl => pl.Id);
            builder.Property(pl => pl.Alias).IsRequired().HasMaxLength(30);
            builder.Property(pl => pl.Name).IsRequired().HasMaxLength(30);
            builder.Property(pl => pl.ExternalId).IsRequired();
        }
    }
}
