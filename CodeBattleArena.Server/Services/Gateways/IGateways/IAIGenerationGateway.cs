using CodeBattleArena.Server.DTO;
using CodeBattleArena.Server.Models;

namespace CodeBattleArena.Server.Services.Gateways.IGateways
{
    public interface IAIGenerationGateway
    {
        Task<AiGeneratedTaskDto> GenerateTaskAsync(RequestGeneratingAITaskDto dto, TaskProgramming? taskProgramming, string? error, CancellationToken ct);
    }
}
