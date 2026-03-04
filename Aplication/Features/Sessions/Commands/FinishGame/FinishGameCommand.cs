
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Sessions.Commands.FinishGame
{
    public record FinishGameCommand(Guid Id) : IRequest<Result<bool>>;
}
