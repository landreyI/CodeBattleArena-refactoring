
using CodeBattleArena.Application.Common.Interfaces;
using CodeBattleArena.Domain.Common;
using MediatR;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Commands.DeleteProgrammingTask
{
    public record DeleteProgrammingTaskCommand(Guid Id) : IRequest<Result<bool>>, ICacheInvalidator
    {
        // Удаляем только одну конкретную карточку по ключу
        public string[] CacheKeys => [Common.CacheKeys.Tasks.Details(Id)];

        // Удаляем ВСЕ списки задач, потому что состав изменился
        public string[] Tags => [Common.CacheKeys.Tasks.ListTag];
    }
}
