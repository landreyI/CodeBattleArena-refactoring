
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.StartGame
{
    public record StartGameCommand(Guid Id) : IRequest<Result<bool>>;
}
