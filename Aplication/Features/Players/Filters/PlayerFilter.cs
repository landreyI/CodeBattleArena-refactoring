
namespace CodeBattleArena.Application.Features.Players.Filters
{
    public class PlayerFilter
    {
        public string? UserName { get; set; }
        public int? Level { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 30;
    }
}
