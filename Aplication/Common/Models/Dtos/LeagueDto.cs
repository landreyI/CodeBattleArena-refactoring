namespace CodeBattleArena.Application.Common.Models.Dtos
{
    public class LeagueDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public int MinWins { get; set; } = default;
        public int? MaxWins { get; set; }

        public List<PlayerDto>? Players { get; set; }
    }
}
