
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Players.Queries.GetPlayer
{
    public record GetPlayerQuery(Guid Id) : IRequest<Result<PlayerDto>>, ICachableRequest
    {
        public string CacheKey => CacheKeys.Players.Details(Id);
        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
