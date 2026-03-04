
using CodeBattleArena.Domain.TestCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeBattleArena.Infrastructure.Persistence.Configurations
{
    public class TestCaseConfiguration : IEntityTypeConfiguration<TestCase>
    {
        public void Configure(EntityTypeBuilder<TestCase> builder)
        {
            builder.HasKey(tc => tc.Id);
            builder.Property(tc => tc.Input).IsRequired();
            builder.Property(tc => tc.ExpectedOutput).IsRequired();

            builder.HasOne(tc => tc.ProgrammingTask)
                   .WithMany(pt => pt.TestCases)
                   .HasForeignKey(tc => tc.ProgrammingTaskId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
