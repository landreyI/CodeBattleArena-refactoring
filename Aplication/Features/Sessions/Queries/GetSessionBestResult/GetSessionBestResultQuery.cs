using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionBestResult
{
    public record GetSessionBestResultQuery(Guid SessionId) : IRequest<Result<PlayerSessionDto>>;
}
