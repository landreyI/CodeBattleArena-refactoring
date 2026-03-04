using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.JoinSession
{
    public record JoinSessionCommand(Guid Id, string? Password) : IRequest<Result<bool>>;
}
