using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.UnjoinSession
{
    public record UnjoinSessionCommand() : IRequest<Result<bool>>;
}
