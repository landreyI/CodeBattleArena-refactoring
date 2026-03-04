
namespace CodeBattleArena.Application.Common.Contracts
{
    /// <summary>
    /// Контракт для очереди в брокер на генерацию задачи через ИИ.
    /// </summary>
    public record GenerateAITaskMessage
    {
        public Guid JobId { get; init; }
        public Guid? ExistingId { get; set; }
        public List<Guid> ProgrammingLangIds { get; init; } = new();
        public string Prompt { get; init; } = string.Empty;
        public string Difficulty { get; init; } = string.Empty;
        public Guid UserId { get; init; }
    }
}
