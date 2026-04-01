using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetLeaguesList
{
    public record GetLeaguesListQuery() : IRequest<Result<IReadOnlyList<LeagueDto>>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Leagues.List();
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
        public string[] Tags => [CacheKeys.Leagues.ListTag];
    }
}