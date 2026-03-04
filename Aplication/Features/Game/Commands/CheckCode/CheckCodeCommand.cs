
using CodeBattleArena.Application.Common.Models;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.Game.Commands.CheckCode
{
    public record CheckCodeCommand(string Code) : IRequest<Result<ExecutionResult>>;
}
