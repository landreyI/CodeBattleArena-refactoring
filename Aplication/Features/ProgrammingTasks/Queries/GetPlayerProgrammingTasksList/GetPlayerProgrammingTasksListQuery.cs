using CodeBattleArena.Application.Common;
using CodeBattleArena.Application.Common.Models.Dtos;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Queries.GetPlayerProgrammingTasksList
{
    public record GetPlayerProgrammingTasksListQuery(Guid PlayerId, ProgrammingTaskFilter Filter)
        : PagedQueryBase<ProgrammingTaskDto>
    {
        public override string CacheKey => CacheKeys.Tasks.PlayerList(PlayerId, Filter);

        public override string[] Tags => [CacheKeys.Tasks.ListTag];
    }
}
