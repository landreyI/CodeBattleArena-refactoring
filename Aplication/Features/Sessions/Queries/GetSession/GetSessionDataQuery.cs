
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Sessions;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public record GetSessionDataQuery(Guid Id) : IRequest<Result<Session>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Sessions.Details(Id);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
