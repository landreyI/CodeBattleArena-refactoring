
using Ardalis.Specification;

namespace CodeBattleArena.Application.Features.ProgrammingTasks.Specifications
{
    public class ProgrammingTaskReadOnlySpec : ProgrammingTaskBaseSpec
    {
        public ProgrammingTaskReadOnlySpec(Guid taskId) : base(taskId)
        {
            AddCommonIncludes();
            Query.AsNoTracking();
        }
    }
}
