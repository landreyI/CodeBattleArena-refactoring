
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Leagues.Queries.GetLeague
{
    public record GetLeagueQuery(Guid Id) : IRequest<Result<LeagueDto>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Leagues.Details(Id);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
