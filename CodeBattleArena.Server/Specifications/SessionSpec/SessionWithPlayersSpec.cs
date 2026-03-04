using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.SessionSpec
{
    public class SessionWithPlayersSpec : SessionByIdSpec
    {
        public SessionWithPlayersSpec(int id) : base(id)
        {
            AddInclude(s => s.PlayerSessions);
        }
    }
}
