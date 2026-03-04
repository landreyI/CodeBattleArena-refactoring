using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Friendships;
using CodeBattleArena.Domain.Items;
using CodeBattleArena.Domain.Leagues;
using CodeBattleArena.Domain.Notifications;
using CodeBattleArena.Domain.PlayerItems;
using CodeBattleArena.Domain.PlayerQuests;
using CodeBattleArena.Domain.Players;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.ProgrammingLanguages;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.QuestParams;
using CodeBattleArena.Domain.QuestRewards;
using CodeBattleArena.Domain.Quests;
using CodeBattleArena.Domain.Rewards;
using CodeBattleArena.Domain.Sessions;
using CodeBattleArena.Domain.TaskLanguages;
using CodeBattleArena.Domain.TestCases;
using CodeBattleArena.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeBattleArena.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<PlayerSession> PlayerSessions { get; set; }
        public DbSet<ProgrammingTask> ProgrammingTasks { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<ProgrammingLang> ProgrammingLanguages { get; set; }
        public DbSet<TaskLanguage> TaskLanguages { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PlayerItem> PlayerItems { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<PlayerQuest> PlayerQuests { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<QuestReward> QuestRewards { get; set; }
        public DbSet<QuestParam> QuestParams { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            /*foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Проверяем, что сущность наследуется от твоей BaseEntity
                // (укажи здесь свой базовый класс или интерфейс)
                if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity<Guid>)))
                {
                    // Находим свойство Id и настраиваем автогенерацию
                    builder.Entity(entityType.ClrType)
                        .Property("Id")
                        .ValueGeneratedOnAdd();
                }
            }*/
        }
    }
}
