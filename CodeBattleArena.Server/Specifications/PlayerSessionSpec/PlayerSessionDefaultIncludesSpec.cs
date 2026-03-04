using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.PlayerSessionSpec
{
    public class PlayerSessionDefaultIncludesSpec : Specification<PlayerSession>
    {
        public PlayerSessionDefaultIncludesSpec() 
        {
            AddInclude(ps => ps.Player);
            AddInclude(ps => ps.Player.ActiveBackground);
            AddInclude(ps => ps.Player.ActiveBorder);
            AddInclude(ps => ps.Player.ActiveAvatar);
            AddInclude(ps => ps.Player.ActiveBadge);
            AddInclude(ps => ps.Player.ActiveTitle);

            AddInclude(ps => ps.Session.LangProgramming);
        }
    }
}
