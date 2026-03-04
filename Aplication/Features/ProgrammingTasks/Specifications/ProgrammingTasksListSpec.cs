
using Ardalis.Specification;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Specifications
{
    public class ProgrammingTasksListSpec : ProgrammingTaskBaseSpec
    {
        // задачи созданные конкртеным игроком
        public ProgrammingTasksListSpec(Guid playerId) : base()
        {
            AddCommonIncludes();
            Query.Where(s => s.AuthorId == playerId)
                 .AsNoTracking()
                 .OrderByDescending(x => x.CreatedAt);
        }

        public ProgrammingTasksListSpec(ProgrammingTaskFilter filter) : base()
        {
            AddCommonIncludes();

            Query.AsNoTracking()
                 .OrderByDescending(x => x.CreatedAt);

            Query.Where(x => x.TaskLanguages.Any(tk => tk.ProgrammingLangId == filter.IdLang), filter.IdLang.HasValue);

            if (!string.IsNullOrWhiteSpace(filter.Difficulty) &&
                Enum.TryParse<Difficulty>(filter.Difficulty, true, out var stateEnum))
            {
                Query.Where(x => x.Difficulty == stateEnum);
            }
        }
    }
}
