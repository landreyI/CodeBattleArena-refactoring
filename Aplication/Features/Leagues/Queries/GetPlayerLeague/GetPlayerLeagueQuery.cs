using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetPlayerLeague
{
    public record GetPlayerLeagueQuery(Guid PlayerId) : IRequest<Result<LeagueDto>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Leagues.PlayerLeague(PlayerId);
        public TimeSpan? Expiration => TimeSpan.FromSeconds(30);
    }
}
