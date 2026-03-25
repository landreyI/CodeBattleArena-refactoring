
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Filters;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerItemsList
{
    public record GetPlayerItemsListQuery(Guid PlayerId, ItemFilter Filter) : PagedQueryBase<PlayerItemDto>
    {
        public override string CacheKey => CacheKeys.Items.PlayerList(PlayerId, Filter);

        public override string[] Tags => [CacheKeys.Items.ListTag];
    }
}
