using Ardalis.Specification;
using CodeBattleArena.Domain.ProgrammingTasks;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Specifications
{
    public abstract class ProgrammingTaskBaseSpec : Specification<ProgrammingTask>
    {
        protected ProgrammingTaskBaseSpec() {}

        protected ProgrammingTaskBaseSpec(Guid taskId) : this()
        {
            Query.Where(s => s.Id == taskId);
        }

        protected void AddCommonIncludes()
        {
            Query.Include(s => s.TaskLanguages)
                    .ThenInclude(tl => tl.ProgrammingLang)
                 .Include(s => s.Author)
                 .Include(s => s.TestCases);
        }
    }
}