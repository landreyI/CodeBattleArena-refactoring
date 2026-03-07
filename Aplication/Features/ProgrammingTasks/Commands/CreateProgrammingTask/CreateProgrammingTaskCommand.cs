
using CodeBattleArena.Application.Common.Interfaces;
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
    ) : IRequest<Result<Guid>>, ICacheInvalidator
    {
        public IReadOnlyList<TestCaseInfo> TestCases { get; init; } = [];
        public IReadOnlyList<TaskLanguageInfo> TaskLanguages { get; init; } = [];

        // Удаляем ВСЕ списки задач, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Tasks.ListTag];
    }
}
