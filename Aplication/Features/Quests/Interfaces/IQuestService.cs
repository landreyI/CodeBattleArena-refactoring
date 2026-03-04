
using CodeBattleArena.Application.Features.Quests.Models;

namespace CodeBattleArena.Application.Features.Quests.Interfaces
{
    public interface IQuestService
    {
        Task ProcessQuestsAsync(QuestTriggerContext context, CancellationToken ct);
    }
}
