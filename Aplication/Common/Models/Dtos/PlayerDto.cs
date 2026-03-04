namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string? AdditionalInformation { get; set; }
        public int Victories { get; set; }
        public int CountGames { get; set; }
        public int Experience { get; set; }
        public int Coins { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? LeagueId { get; set; }
        public virtual LeagueDto? League { get; set; }
    }
}
