
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.KickOutSession
{
    public record KickOutSessionCommand(Guid SessionId, Guid PlayerId) : IRequest<Result<bool>>;
}
