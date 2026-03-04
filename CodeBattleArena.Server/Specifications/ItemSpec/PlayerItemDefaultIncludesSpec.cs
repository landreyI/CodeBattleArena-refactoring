using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.ItemSpec
{
    public class PlayerItemDefaultIncludesSpec : Specification<PlayerItem>
    {
        public PlayerItemDefaultIncludesSpec()
        {
            AddInclude(ps => ps.Player);
            AddInclude(ps => ps.Item);
        }
    }
}
