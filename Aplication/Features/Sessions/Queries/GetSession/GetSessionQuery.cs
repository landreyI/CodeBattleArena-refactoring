using CodeBattleArena.Application.Features.Sessions.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Queries.GetSession
{
    public record GetSessionQuery(Guid Id) : IRequest<Result<SessionDtoAndIsEdit>>;
}
