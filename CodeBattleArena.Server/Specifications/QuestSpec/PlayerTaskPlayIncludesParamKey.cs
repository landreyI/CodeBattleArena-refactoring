using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class PlayerTaskPlayIncludesParamKey : Specification<PlayerTaskPlay>
    {
        public PlayerTaskPlayIncludesParamKey()
        {
            AddInclude(s => s.TaskPlay);
            AddInclude(s => s.TaskPlay.TaskPlayParams);
        }
    }
}
