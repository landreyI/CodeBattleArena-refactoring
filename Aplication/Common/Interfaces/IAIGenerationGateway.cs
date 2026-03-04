using CodeBattleArena.Application.Features.AI.Models;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.TaskLanguages;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IAIGenerationGateway
    {
        Task<Result<AiGeneratedTaskDto>> GenerateTaskMetadataAndTestsAsync(RequestGeneratingAITask request, CancellationToken ct = default);
        Task<Result<AiTaskLanguageDto>> GenerateLanguageCodeAsync(string userPrompt, string languageName, TaskLanguage? existingLang = default, string? errorApi = default, CancellationToken ct = default);
        Task<Result<AiGeneratedTaskDto>> GenerateTaskAsync(RequestGeneratingAITask request, Dictionary<Guid, string>? errorsApi = default, CancellationToken ct = default);
    }
}
