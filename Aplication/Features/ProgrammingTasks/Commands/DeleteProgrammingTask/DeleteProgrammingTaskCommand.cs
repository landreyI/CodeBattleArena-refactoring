
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.DeleteProgrammingTask
{
    public record DeleteProgrammingTaskCommand(Guid Id) : IRequest<Result<bool>>;
}
