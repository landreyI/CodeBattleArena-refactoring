using CodeBattleArena.Server.Specifications.SessionSpec;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class TaskPlayListsIncludesSpec : TaskPlayDefaultIncludesSpec
    {
        public TaskPlayListsIncludesSpec() : base()
        {
            AddInclude(s => s.TaskPlayRewards);
            AddInclude(s => s.PlayerTaskPlays);
        }
    }
}
