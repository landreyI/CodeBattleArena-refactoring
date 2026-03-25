
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Items.Filters;

namespace CodeBattleArena.Application.Features.Items.Queries.GetItemsList
{
    public record GetItemsListQuery(ItemFilter Filter)
    : PagedQueryBase<ItemDto>
    {
        public override string CacheKey => CacheKeys.Items.List(Filter);

        public override string[] Tags => [CacheKeys.Items.ListTag];
    }
}
