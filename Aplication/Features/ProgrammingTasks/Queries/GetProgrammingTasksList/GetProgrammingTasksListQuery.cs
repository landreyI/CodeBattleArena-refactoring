
using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetProgrammingTasksList
{
    public record GetProgrammingTasksListQuery(ProgrammingTaskFilter Filter)
    : PagedQueryBase<ProgrammingTaskDto>
    {
        public override string CacheKey => CacheKeys.Tasks.List(Filter);

        public override string[] Tags => [CacheKeys.Tasks.ListTag];
    }
}
