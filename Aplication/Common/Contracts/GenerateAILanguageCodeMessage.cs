
namespace CodeBattleArena.Application.Common.Contracts
{
    public class GenerateAILanguageCodeMessage
    {
        public Guid TaskId { get; set; }
        public Guid LanguageId { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public Guid JobId { get; set; }
        public Guid UserId { get; init; }
    }
}
