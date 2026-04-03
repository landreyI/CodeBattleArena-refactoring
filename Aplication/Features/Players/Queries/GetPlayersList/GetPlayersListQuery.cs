
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Players.Filters;

namespace CodeBattleArena.Application.Features.Players.Queries.GetPlayersList
{
    public record GetPlayersListQuery(PlayerFilter Filter)
    : PagedQueryBase<PlayerDto>
    {
        public override string CacheKey => CacheKeys.Players.List(Filter);

        public override string[] Tags => [CacheKeys.Players.ListTag];
        public override TimeSpan? Expiration => TimeSpan.FromMinutes(2);
    }
}
