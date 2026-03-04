using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.Specifications.QuestSpec
{
    public class PlayerTaskPlayByTypeSpec : PlayerTaskPlayIncludesParamKey
    {
        public PlayerTaskPlayByTypeSpec(TaskType? taskType = null) : base()
        {
            if (taskType.HasValue)
                Criteria = pt => pt.TaskPlay.Type == taskType;
        }
    }
}
