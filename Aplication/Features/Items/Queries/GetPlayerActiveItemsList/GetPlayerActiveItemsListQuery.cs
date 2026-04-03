
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Items.Queries.GetPlayerActiveItemsList
{
    public record GetPlayerActiveItemsListQuery(Guid PlayerId) : IRequest<Result<IReadOnlyList<ItemDto>>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Items.PlayerActiveList(PlayerId);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
        public string[] Tags => [CacheKeys.Leagues.ListTag];
    }
}
