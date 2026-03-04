using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionDefaultIncludesSpec : Specification<Session>
    {
        public SessionDefaultIncludesSpec()
        {
            AddInclude(s => s.LangProgramming);
            AddInclude(s => s.TaskProgramming);
        }
    }
}
