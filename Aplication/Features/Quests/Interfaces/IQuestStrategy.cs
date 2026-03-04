using CodeBattleArena.Application.Features.Quests.Models;
using CodeBattleArena.Domain.Enums;
using CodeBattleArena.Domain.PlayerQuests;

namespace CodeBattleArena.Application.Features.Quests.Interfaces
{
    public interface IQuestStrategy
    {
        TaskType SupportedTaskType { get; }

        bool CanHandle(GameEventType eventType);

        Task ProcessAsync(PlayerQuest playerQuest, QuestTriggerContext context, CancellationToken ct);
    }
}
