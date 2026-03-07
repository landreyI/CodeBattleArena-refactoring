using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.Sessions.Filters;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionsList
{
    public record GetSessionsListQuery(SessionFilter Filter)
    : PagedQueryBase<SessionDto>
    {
        public override string CacheKey => CacheKeys.Sessions.List(Filter);

        public override string[] Tags => [CacheKeys.Sessions.ListTag];
    }
}
