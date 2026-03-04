using CodeBattleArena.Domain.ProgrammingTasks;

namespace CodeBattleArena.Application.Features.AI.Models
{
    public class RequestGeneratingAITask
    {
        public string Prompt { get; init; } = string.Empty;
        public Dictionary<Guid, string> ProgrammingLanguages { get; init; } = new();
        public string Difficulty { get; init; } = string.Empty;
        public ProgrammingTask? ExistingTask { get; init; }
    }
}
