
namespace CodeBattleArena.Domain.Players.Value_Objects
{
    public record PlayerStats
    {
        public int Victories { get; init; } = 0;
        public int CountGames { get; init; } = 0;
        public int Experience { get; init; } = 0;

        public int Level => (Experience / 1000) + 1;
        public PlayerStats AddVictory(int xp) =>
            this with { Victories = Victories + 1, CountGames = CountGames + 1, Experience = Experience + xp };

        public PlayerStats AddLoss(int xp) =>
            this with { CountGames = CountGames + 1, Experience = Experience + xp };
    }
}
