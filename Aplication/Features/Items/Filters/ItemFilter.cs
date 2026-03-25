

namespace CodeBattleArena.Application.Features.Items.Filters
{
    public class ItemFilter
    {
        public string? Type { get; set; }
        public int? Coin { get; set; }
        public bool? IsCoinDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
