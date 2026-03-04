
using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSessionCompletionCount
{
    public record GetSessionCompletionCountQuery(Guid SessionId) : IRequest<Result<CompletionCount>>;
}
