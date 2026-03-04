
namespace CodeBattleArena.Domain.Quests.Value_Objects
{
    public record QuestRecurrence(bool IsRepeatable, int? RepeatAfterDays = default);
}
