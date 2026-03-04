
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.DeleteSession
{
    public record DeleteSessionCommand(Guid Id) : IRequest<Result<bool>>;
}
