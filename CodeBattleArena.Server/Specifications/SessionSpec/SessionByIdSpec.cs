using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionByIdSpec : SessionDefaultIncludesSpec
    {
        public SessionByIdSpec(int id) : base()
        {
            Criteria = s => s.IdSession == id;
        }
    }
}
