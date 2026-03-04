
namespace CodeBattleArena.Domain.PlayerSessions.Value_Objects
{
    public record SubmissionResult(string? CodeText = default, string? Time = default, int? Memory = default, DateTime? FinishTask = default);
}
