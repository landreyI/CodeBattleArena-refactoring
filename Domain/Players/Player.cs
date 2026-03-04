
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Friendships;
using CodeBattleArena.Domain.Leagues;
using CodeBattleArena.Domain.PlayerItems;
using CodeBattleArena.Domain.PlayerQuests;
using CodeBattleArena.Domain.Players.Value_Objects;
using CodeBattleArena.Domain.PlayerSessions;
using CodeBattleArena.Domain.ProgrammingTasks;

namespace CodeBattleArena.Domain.Players
{
    public class Player : BaseEntity<Guid>
    {
        public string IdentityId { get; private set; }
        public PlayerProfile Profile { get; private set; }
        public PlayerStats Stats { get; private set; }
        public Wallet Wallet { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        public Guid? LeagueId { get; private set; }
        public virtual League? League { get; private set; }

        private readonly List<PlayerSession> _playerSessions = new();
        public virtual ICollection<PlayerSession>? PlayerSessions => _playerSessions.AsReadOnly();

        private readonly List<ProgrammingTask> _tasks = new();
        public virtual ICollection<ProgrammingTask> Tasks => _tasks.AsReadOnly();

        private readonly List<PlayerItem> _playerItems = new();
        public virtual ICollection<PlayerItem> PlayerItems => _playerItems.AsReadOnly();

        private readonly List<PlayerQuest> _playerQuests = new();
        public virtual ICollection<PlayerQuest> PlayerQuests => _playerQuests.AsReadOnly();

        // Исходящие заявки
        private readonly List<Friendship> _sentRequests = new();
        public virtual ICollection<Friendship> SentRequests => _sentRequests.AsReadOnly();

        // Входящие заявки
        private readonly List<Friendship> _receivedRequests = new();
        public virtual ICollection<Friendship> ReceivedRequests => _receivedRequests.AsReadOnly();

        private Player() { } // Для EF

        private Player(string identityId, string name)
        {
            IdentityId = identityId;
            Profile = new PlayerProfile(name);
            Stats = new PlayerStats();
            Wallet = new Wallet();
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Player> Create(string identityId, string name)
        {
            if (string.IsNullOrWhiteSpace(identityId))
                return Result<Player>.Failure(new Error("player.no_identity", "Identity link is required."));

            if (string.IsNullOrWhiteSpace(name))
                return Result<Player>.Failure(new Error("player.no_name", "Player name cannot be empty."));

            return Result<Player>.Success(new Player(identityId, name));
        }

        public Result UpdateProfile(string? name = default, string? photoUrl = default, string? info = default)
        {
            if (name != null && string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error("player.invalid_name", "Name cannot be empty."));

            Profile = new PlayerProfile(name ?? Profile.Name, photoUrl ?? Profile.PhotoUrl, info ?? Profile.AdditionalInformation);
            return Result.Success();
        }

        public Result CompleteMatch(bool won, int xpReward, int coinReward)
        {
            if (xpReward < 0 || coinReward < 0)
                return Result.Failure(new Error("player.invalid_reward", "Rewards cannot be negative."));

            Stats = won ? Stats.AddVictory(xpReward) : Stats.AddLoss(xpReward);
            Wallet = Wallet.Add(coinReward);

            return Result.Success();
        }

        public Result UpdateRefreshToken(string token, int days)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Result.Failure(new Error("player.invalid_token", "Refresh token cannot be empty."));

            if (days <= 0)
                return Result.Failure(new Error("player.invalid_expiry", "Expiry days must be positive."));

            RefreshToken = token;
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(days);

            return Result.Success();
        }
    }
}
