
using Ardalis.Specification;
using CodeBattleArena.Application.Features.ProgrammingTasks.Filters;
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Specifications
{
    public class ProgrammingTasksListSpec : ProgrammingTaskBaseSpec
    {
        // Конструктор для общего списка
        public ProgrammingTasksListSpec(ProgrammingTaskFilter filter) : base()
        {
            AddCommonIncludes();
            Query.AsNoTracking().OrderByDescending(x => x.CreatedAt);

            ApplyFilter(filter);
        }

        // Конструктор для списка конкретного игрока
        public ProgrammingTasksListSpec(Guid playerId, ProgrammingTaskFilter filter) : base()
        {
            AddCommonIncludes();
            Query.AsNoTracking().OrderByDescending(x => x.CreatedAt);

            Query.Where(s => s.AuthorId == playerId);

            ApplyFilter(filter);
        }

        private void ApplyFilter(ProgrammingTaskFilter filter)
        {
            Query.Where(x => x.TaskLanguages.Any(tk => tk.ProgrammingLangId == filter.IdLang), filter.IdLang.HasValue);

            if (!string.IsNullOrWhiteSpace(filter.Difficulty) &&
                Enum.TryParse<Difficulty>(filter.Difficulty, true, out var stateEnum))
            {
                Query.Where(x => x.Difficulty == stateEnum);
            }

            Query.Skip((filter.PageNumber - 1) * filter.PageSize)
                 .Take(filter.PageSize);
        }
    }
}
