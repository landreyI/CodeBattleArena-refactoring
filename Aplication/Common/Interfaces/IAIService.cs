
using CodeBattleArena.Application.Common.Contracts;
using CodeBattleArena.Domain.Common;
using CodeBattleArena.Domain.ProgrammingTasks;
using CodeBattleArena.Domain.TaskLanguages;

namespace CodeBattleArena.Application.Common.Interfaces
{
    public interface IAIService
    {
        Task<Result<ProgrammingTask>> GenerateTaskMetadataAndTestsAsync(GenerateAITaskMessage generateAITaskMessage, CancellationToken ct = default);
        Task<Result<(ProgrammingTask, TaskLanguage)>> GenerateLanguageCodeAsync(GenerateAILanguageCodeMessage generateAILanguageCodeMessage, CancellationToken ct = default);
        //Task<Result<ProgrammingTask>> GenerateAIProgrammingTaskAsync(ProcessAIGenerationProgrammingTaskCommand request, CancellationToken ct = default);

    }
}
