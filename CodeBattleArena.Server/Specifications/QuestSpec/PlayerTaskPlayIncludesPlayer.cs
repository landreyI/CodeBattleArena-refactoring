using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class PlayerTaskPlayIncludesPlayer : Specification<PlayerTaskPlay>
    {
        public PlayerTaskPlayIncludesPlayer()
        {
            AddInclude(s => s.Player);
        }
    }
}
