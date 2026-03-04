
using CodeBattleArena.Domain.Enums;

namespace CodeBattleArena.Application.Features.Quests.Models
{
    public record QuestTriggerContext(
    Guid PlayerId,
    GameEventType EventType,
    Dictionary<string, object>? Metadata = null);
}
