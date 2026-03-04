using CodeBattleArena.Server.Enums;

namespace CodeBattleArena.Server.QuestSystem.Interfaces
{
    public interface IQuestTriggerHandler
    {
        bool CanHandle(GameEventType eventType);
        Task HandleAsync(GameEventContext context, CancellationToken cancellationToken, bool commit = true);
    }
}
