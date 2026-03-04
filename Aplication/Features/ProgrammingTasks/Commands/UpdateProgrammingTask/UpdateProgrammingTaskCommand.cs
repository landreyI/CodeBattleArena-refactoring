
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.ProgrammingTasks.Value_Objects;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.UpdateProgrammingTask
{
    public record UpdateProgrammingTaskCommand(
        Guid Id,
        string? Name = null,
        string? Description = null,
        Difficulty? Difficulty = null
    ) : IRequest<Result<bool>>
    {
        public IReadOnlyList<TestCaseInfo>? TestCases { get; init; }
        public IReadOnlyList<TaskLanguageInfo>? TaskLanguages { get; init; }
    }
}
