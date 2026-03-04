
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.InviteSession
{
    public record InviteSessionCommand(Guid SessionId, List<Guid> PlayerIds) : IRequest<Result<bool>>;
}
