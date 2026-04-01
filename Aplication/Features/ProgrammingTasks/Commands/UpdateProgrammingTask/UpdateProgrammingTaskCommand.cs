
using CodeBattleArena.Application.Common.Interfaces;
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
    ) : IRequest<Result<bool>>, ICacheInvalidator, IStaffRequest
    {
        public IReadOnlyList<TestCaseInfo>? TestCases { get; init; }
        public IReadOnlyList<TaskLanguageInfo>? TaskLanguages { get; init; }

        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Tasks.Details(Id)];

        // Удаляем ВСЕ списки задач, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Tasks.ListTag];
    }
}
