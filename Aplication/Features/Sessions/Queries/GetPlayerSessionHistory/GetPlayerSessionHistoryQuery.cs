using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetPlayerSessionHistory
{
    public record GetPlayerSessionHistoryQuery(Guid PlayerId) : IRequest<Result<List<SessionDto>>>;
}
