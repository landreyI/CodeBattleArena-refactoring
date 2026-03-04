
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.ProgrammingTasks.Value_Objects;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.CreateProgrammingTask
{
    public record CreateProgrammingTaskCommand(
        string Name,
        string Description,
        Difficulty Difficulty
    ) : IRequest<Result<Guid>>
    {
        public IReadOnlyList<TestCaseInfo> TestCases { get; init; } = [];
        public IReadOnlyList<TaskLanguageInfo> TaskLanguages { get; init; } = [];
    }
}
