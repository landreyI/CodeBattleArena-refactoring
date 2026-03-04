
namespace CodeBattleArena.Domain.ProgrammingTasks.Value_Objects
{
    public record TaskLanguageInfo(
        Guid ProgrammingLangId,
        string Preparation,
        string VerificationCode,
        bool IsGeneratedAI = false
    );
}
