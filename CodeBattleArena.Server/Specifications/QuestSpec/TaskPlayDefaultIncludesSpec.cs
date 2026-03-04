using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class TaskPlayDefaultIncludesSpec : Specification<TaskPlay>
    {
        public TaskPlayDefaultIncludesSpec()
        {
            AddInclude(s => s.TaskPlayParams);
        }
    }
}
