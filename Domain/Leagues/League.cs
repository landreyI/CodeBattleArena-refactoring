using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Players;

namespace CodeBattleArena.Domain.Leagues
{
    public class League : BaseEntity<Guid>
    {
        public string Name { get; private set; }
        public string? PhotoUrl { get; private set; }
        public int MinWins { get; private set; }
        public int? MaxWins { get; private set; }

        private readonly List<Player> _players = new();
        public virtual ICollection<Player> Players => _players.AsReadOnly();

        protected League() { } // Для EF Core

        private League(string name, int minWins, int? maxWins, string? photoUrl)
        {
            Name = name;
            MinWins = minWins;
            MaxWins = maxWins;
            PhotoUrl = photoUrl;
        }

        public static Result<League> Create(string name, int minWins, int? maxWins = default, string? photoUrl = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<League>.Failure(new Error("league.invalid_name", "League name cannot be empty."));

            if (minWins < 0)
                return Result<League>.Failure(new Error("league.invalid_min_wins", "Minimum wins cannot be negative."));

            if (maxWins.HasValue && minWins > maxWins.Value)
                return Result<League>.Failure(new Error("league.invalid_range", "Minimum wins cannot be greater than maximum wins."));

            return Result<League>.Success(new League(name, minWins, maxWins, photoUrl));
        }

        public bool IsEligible(int victories)
        {
            bool meetsMin = victories >= MinWins;
            bool meetsMax = !MaxWins.HasValue || victories <= MaxWins.Value;

            return meetsMin && meetsMax;
        }

        public Result UpdateRequirements(int minWins, int? maxWins = default)
        {
            if (minWins < 0)
                return Result.Failure(new Error("league.invalid_min_wins", "Minimum wins cannot be negative."));

            if (maxWins.HasValue && minWins > maxWins.Value)
                return Result.Failure(new Error("league.invalid_range", "Incorrect win limits: Min > Max."));

            MinWins = minWins;
            MaxWins = maxWins;

            return Result.Success();
        }
    }
}