using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionInfo
{
    public class GetPlayerSessionInfoQuery : IRequest<Result<PlayerSessionDto?>>
    {
        public Guid SessionId { get; }
        public Guid PlayerId { get; }
        public GetPlayerSessionInfoQuery(Guid sessionId, Guid playerId)
        {
            SessionId = sessionId;
            PlayerId = playerId;
        }
    }
}
