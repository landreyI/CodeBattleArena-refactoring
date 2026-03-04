using CodeBattleArena.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CodeBattleArena.Server.Data
{
    public class AppDBContext : IdentityDbContext<Player>
    {
        public AppDBContext(DbContextOptions<AppDBContext> option) : base(option) { }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<PlayerSession> PlayersSession { get; set; }
        public DbSet<TaskProgramming> TasksProgramming { get; set; }
        public DbSet<InputData> InputData { get; set; }
        public DbSet<TaskInputData> TaskInputData { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<LangProgramming> LangProgrammings { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<TaskPlay> TasksPlay { get; set; }
        public DbSet<TaskPlayParam> TaskPlayParams { get; set; }
        public DbSet<PlayerTaskPlay> PlayerTaskPlays { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<TaskPlayReward> TaskPlayRewards { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PlayerItem> PlayerItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // enum => string
            modelBuilder.Entity<Session>()
                .Property(g => g.State)
                .HasConversion<string>(); 

            modelBuilder.Entity<TaskProgramming>()
                .Property(g => g.Difficulty)
                .HasConversion<string>();

            modelBuilder.Entity<Item>()
                .Property(i => i.Type)
                .HasConversion<string>();

            modelBuilder.Entity<TaskPlay>()
                .Property(i => i.Type)
                .HasConversion<string>();

            modelBuilder.Entity<TaskPlayParam>()
                .Property(i => i.ParamKey)
                .HasConversion<string>();


            modelBuilder.Entity<PlayerSession>()
                .HasKey(b => new { b.IdPlayer, b.IdSession });

            modelBuilder.Entity<PlayerSession>()
                .HasOne(b => b.Player)
                .WithMany(u => u.PlayerSessions)
                .HasForeignKey(b => b.IdPlayer);

            modelBuilder.Entity<PlayerSession>()
                .HasOne(b => b.Session)
                .WithMany(p => p.PlayerSessions)
                .HasForeignKey(b => b.IdSession);


            modelBuilder.Entity<Session>()
                .HasOne(s => s.TaskProgramming)
                .WithMany(t => t.Sessions)
                .HasForeignKey(s => s.TaskId);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.LangProgramming)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.LangProgrammingId);


            modelBuilder.Entity<TaskProgramming>()
                .HasOne(s => s.LangProgramming)
                .WithMany(u => u.TasksProgramming)
                .HasForeignKey(s => s.LangProgrammingId);

            modelBuilder.Entity<TaskProgramming>()
                .HasOne(s => s.Player)
                .WithMany(u => u.TaskProgrammings)
                .HasForeignKey(s => s.IdPlayer);


            modelBuilder.Entity<TaskInputData>()
                .HasKey(ti => new { ti.IdTaskProgramming, ti.IdInputDataTask });

            modelBuilder.Entity<TaskInputData>()
                .HasOne(ti => ti.TaskProgramming)
                .WithMany(t => t.TaskInputData)
                .HasForeignKey(ti => ti.IdTaskProgramming);

            modelBuilder.Entity<TaskInputData>()
                .HasOne(ti => ti.InputData)
                .WithMany(i => i.TaskInputData)
                .HasForeignKey(ti => ti.IdInputDataTask);



            modelBuilder.Entity<TaskPlayParam>()
                .HasIndex(p => new { p.TaskPlayId, p.ParamKey })
                .IsUnique();

            modelBuilder.Entity<TaskPlayParam>()
                .HasOne(p => p.TaskPlay)
                .WithMany(t => t.TaskPlayParams)
                .HasForeignKey(p => p.TaskPlayId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlayerTaskPlay>()
                .HasKey(b => b.IdPlayerTaskPlay);

            modelBuilder.Entity<PlayerTaskPlay>()
                .HasOne(b => b.Player)
                .WithMany(u => u.PlayerTaskPlays)
                .HasForeignKey(b => b.PlayerId);

            modelBuilder.Entity<PlayerTaskPlay>()
                .HasOne(b => b.TaskPlay)
                .WithMany(p => p.PlayerTaskPlays)
                .HasForeignKey(b => b.TaskPlayId);

            modelBuilder.Entity<TaskPlayReward>()
                .HasKey(x => new { x.TaskPlayId, x.RewardId });

            modelBuilder.Entity<TaskPlayReward>()
                .HasOne(tr => tr.TaskPlay)
                .WithMany(t => t.TaskPlayRewards)
                .HasForeignKey(tr => tr.TaskPlayId);

            modelBuilder.Entity<TaskPlayReward>()
                .HasOne(tr => tr.Reward)
                .WithMany()
                .HasForeignKey(tr => tr.RewardId);


            // Конфигурация активных предметов в Player
            modelBuilder.Entity<Player>()
                .HasOne(p => p.ActiveBackground)
                .WithMany() // Item не имеет обратной коллекции для ActiveBackground
                .HasForeignKey(p => p.ActiveBackgroundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.ActiveAvatar)
                .WithMany()
                .HasForeignKey(p => p.ActiveAvatarId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.ActiveBadge)
                .WithMany()
                .HasForeignKey(p => p.ActiveBadgeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.ActiveBorder)
                .WithMany()
                .HasForeignKey(p => p.ActiveBorderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.ActiveTitle)
                .WithMany()
                .HasForeignKey(p => p.ActiveTitleId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<PlayerItem>()
                .HasKey(f => new { f.IdPlayer, f.IdItem });

            modelBuilder.Entity<PlayerItem>()
                .HasOne(i => i.Item)
                .WithMany(i => i.PlayerItems)
                .HasForeignKey(i => i.IdItem)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerItem>()
                .HasOne(i => i.Player)
                .WithMany(i => i.PlayerItems)
                .HasForeignKey(i => i.IdPlayer)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Requester)
                .WithMany(p => p.Friends1)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friend>()
                .HasOne(f => f.Addressee)
                .WithMany(p => p.Friends2)
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Player1)
                .WithMany(p => p.ChatsAsUser1)
                .HasForeignKey(c => c.IdPlayer1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Player2)
                .WithMany(p => p.ChatsAsUser2)
                .HasForeignKey(c => c.IdPlayer2)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.IdChat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(p => p.Messages)
                .HasForeignKey(m => m.IdSender)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasIndex(c => new { c.IdPlayer1, c.IdPlayer2 })
                .IsUnique();
        }
    }
}
