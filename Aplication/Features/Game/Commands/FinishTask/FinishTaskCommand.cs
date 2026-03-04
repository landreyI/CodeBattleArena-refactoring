
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.Commands.FinishTask
{
    public record FinishTaskCommand() : IRequest<Result<bool>>;
}
