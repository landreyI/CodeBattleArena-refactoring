using CodeBattleArena.Server.Filters;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionsByFilter : SessionDefaultIncludesSpec
    {
        public SessionsByFilter(IFilter<Session>? filter) : base()
        {
            Filter = filter;
            AddInclude(s => s.PlayerSessions);
        }
    }
}
