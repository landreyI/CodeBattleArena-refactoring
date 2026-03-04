using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.QuestSystem
{
    public class GameEventContext
    {
        public GameEventType EventType { get; set; }
        public string PlayerId { get; set; }
    }
}
