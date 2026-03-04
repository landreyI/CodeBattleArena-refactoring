namespace CodeBattleArena.Server.DTO.ModelsDTO
{
    public class LeagueDto
    {
        public int? IdLeague { get; set; }
        public string Name { get; set; }
        public string? PhotoUrl { get; set; }
        public int MinWins { get; set; }
        public int? MaxWins { get; set; }
    }
}
